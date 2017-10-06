using Nsc.Wu.Common;
using Nsc.Wu.Common.Push;
using Nsc.Wu.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.BLL.Service
{
    public class CommunicationService
    {
        public void SendPush(string deviceId, int userid, string action, string platform, string from, string to, string data)
        {
            switch (platform)
            {
                case "IOS":
                    sendIOS(action, userid, deviceId, from, to, data);
                    break;
                default:
                    sendAndroid(action, userid, deviceId, from, to, data);
                    break;

            }
        }
        public void sendIOS(string action, int userid, string deviceId, string from, string to, string data)
        {
            string message = "";
            string type = "";
            PushApi push = new PushApi();
            switch (action)
            {
                case "request":
                    message = from + " wants to participate to your party " + data;
                    type = "REQUEST_RECEIVED";
                    break;
                case "invite":
                    message = from + " sent you an invitation to his party " + data;
                    type = "INVITATION_RECEIVED";
                    break;
                case "acceptrequest":
                    message = $"{from} has accepted your request to his party {data}";
                    type = "REQUEST_ACCEPTED";
                    break;

                case "acceptinvite":
                    message = $"{from} has accepted your invitation to his party {data}";
                    type = "INVITATION_ACCEPTED";
                    break;
            }
            push.IOSPush(deviceId, type, message);
            if(userid!=0)
            SaveNotification(deviceId, userid, type, message);

        }
        public void sendAndroid(string action, int userid, string deviceId, string from, string to, string data)
        {
            string type = "";
            string message = "";
            PushApi push = new PushApi();
            switch (action)
            {
                case "request":
                    message = from + " wants to participate to your party " + data;
                    type = "REQUEST_RECEIVED";
                    break;
                case "invite":
                    message = from + " sent you an invitation to his party " + data;
                    type = "INVITATION_RECEIVED";
                    break;
                case "acceptrequest":
                    message = $"{from} has accepted your request to his party {data}";
                    type = "REQUEST_ACCEPTED";
                    break;

                case "acceptinvite":
                    message = $"{from} has accepted your invitation to his party {data}";
                    type = "INVITATION_ACCEPTED";
                    break;
            }
            push.AndroidPush(deviceId, type, message);
            if (userid != 0)
                SaveNotification(deviceId, userid, type, message);
        }
        public ServiceResponseWithResult MarkNotificationAsRead(long notificationId)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            Model.Communication data =  context.Communications.FirstOrDefault(n => n.CommunicationId == notificationId);
            data.LastUpdatedUtcDateTime = DateTime.UtcNow;
            data.IsRead = true;
            context.SaveChanges();

            return new ServiceResponseWithResult(true, "success", null);
        }
        public List<NotificationDetailDto> UserNotifications(int userid, int offset, int size)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            var query = context.Communications.Where(c => c.CustomerId == userid)
                .OrderByDescending(o => o.CreatedUtcDateTime)
                .Skip((offset - 1) * size).Take(size);



            return query.Select(d => new NotificationDetailDto()
            {
                body = d.CommunicationDatas.FirstOrDefault().DateValue,
                type = d.CommunicationDatas.FirstOrDefault().DataKey,
                id = d.CommunicationId,
                isRead = (bool)d.IsRead
            }).ToList();
        }
        private void SaveNotification(string deviceId, int userid, string action, string message)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            context.Communications.Add(new Model.Communication()
            {

                CreatedUtcDateTime = DateTime.UtcNow,
                DevicePushId = deviceId,
                IsRead = false,
                CustomerId = userid,
                CommunicationDatas = new List<Model.CommunicationData>
                {
                    new Model.CommunicationData() {
                        DataKey = action,
                        DateValue = message,
                        CreatedUtcDateTime = DateTime.UtcNow
                    }
                }


            });
            context.SaveChanges();
        }
    }
}
