using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Common.Push
{
    public class PushApi
    {
        public PushApi() { }
        public void AndroidPush(string regid,string type,string message)
        {
            // Configuration
            var config = new GcmConfiguration("180276633609", "AAAAKflTIAk:APA91bGPlwTWWGPkarFwK_AZxxSfw22DbFVaiOi59nBnmqgAsfSQKWqnOtr_tQKx95Q2pqF5ilcaTBMRRRKe4lTVgSu9AN7tl_SQp8stUV2n1jQDpzC5VgDCTc83txYI7N3MxxJpssqV", null);

            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Console.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                        if (!String.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                    }
                    else
                    {
                        Console.WriteLine("GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("GCM Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start();
            var json = JObject.Parse("{ \"Message\" : \'" + message + "\'," + "\'Type':\'" + type + "'}");
            //foreach (var regId in MY_REGISTRATION_IDS)
            //{
            // Queue a notification to send
            gcmBroker.QueueNotification(new GcmNotification
                {
                    RegistrationIds = new List<string> {
                        regid
            //"dBmhTFW3iKc:APA91bGro9nr0E0-f7hQ4HdEhyuAusff0IaplvhKpa3gRDQHaeLFaU8e0l3QxSsPG-dv9GtvOj9Y5-mwfm1J8dD1UVkRgIIgTS_gdXr_TSALK4QWqP2etDJLJawH_VvA-qMupjXMMmE9"
        },
                    Data = json
                });
            //}

            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop();
        }
        public void IOSPush(string regid, string type, string message)
        {
            string src = ConfigurationManager.AppSettings["Env"].ToString();
            var appleCertificate = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ioscertpath"].ToString());
            // Configuration (NOTE: .pfx can also be used here)
            ApnsConfiguration config = null;
           
            if(src.Equals("Dev"))
            {
               // appleCertificate = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ioscertpath"].ToString());
                // Configuration (NOTE: .pfx can also be used here)
                config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, appleCertificate, "warmup123");

            }
            else
            {
                config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, appleCertificate, "warmup123");
            }
            // Create a new broker
            var apnsBroker = new ApnsServiceBroker(config);

            // Wire up events
            apnsBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = (ApnsNotificationException)ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException			
                        Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apnsBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("Apple Notification Sent!");
            };

            // Start the broker
            apnsBroker.Start();
            var obj = JObject.Parse("{\"aps\":{\"alert\":{\"body\":'"+message+"',\"type\": '"+type+"'}, \"badge\":1, \"sound\":\"default\", \"content-available\":1}}");
            //foreach (var deviceToken in MY_DEVICE_TOKENS)
            //{
            // Queue a notification to send
            apnsBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = regid,
                    Payload = obj
                });
            //}

            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            apnsBroker.Stop();
        }
    }
}
