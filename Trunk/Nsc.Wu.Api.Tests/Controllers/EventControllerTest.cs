using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nsc.Wu.Common;
using RestSharp;
using Newtonsoft.Json;

namespace Nsc.Wu.Api.Tests.Controllers
{
    [TestClass]
    public class EventControllerTest
    {
        string _serviceEndpoint = "http://localhost:56527/api/";
       // string _serviceEndpoint = "http://193.42.156.14:5090/api/";
        [TestMethod]
        public void Create()
        {
            // EventController account = new AccountController();
            //  var result = account.Register(new Dto.User() { FirstName = "kamran", LastName = "Jamil", Email = "kamran@mailinator.com", TypeId = 1, PasswordHash = "123456" });;
            ServiceResponseWithResult res = JsonConvert.DeserializeObject<ServiceResponseWithResult>(CreateEvent(new Dto.Event() {CreatedBy=37,EventLocations=new System.Collections.Generic.List<Dto.EventLocation>() { new Dto.EventLocation() { Lat= "35.766881", Lng= "-5.852423" } } ,Title = new Random().Next(0, 800) + "Music", Address = new Random().Next(0,800)+" Street", Description = new Random().Next(0, 800) + "khj jkhj dfdfdfd", EventEndDateUnix = (DateTime.Parse("08-06-2017")).ToUnixTimestamp(), EventStartDateUnix = DateTime.Parse("01-07-2018").ToUnixTimestamp(),PaymentFee="10",MaxParticipants=9 }).Content);
            Assert.IsTrue(res.IsSuccess);
        }
        public IRestResponse CreateEvent(Dto.Event events)
        {
            string url = _serviceEndpoint + "event";
            RestClient restClient = new RestClient(url);

            var request = new RestRequest("create", Method.POST);

            request.AddHeader("ApplicationKey", "BF1A4A1C-A55D-49E1-AFCD-52A407095DEF");
            request.AddJsonBody(events);

            IRestResponse response = restClient.Execute(request);

            return response;

        }
    }
   
}
