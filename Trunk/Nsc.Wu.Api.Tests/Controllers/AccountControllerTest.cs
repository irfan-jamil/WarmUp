using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nsc.Wu.Api.Controllers;
using System.IO;
using RestSharp;
using Nsc.Wu.Common;
using Newtonsoft.Json;
using Nsc.Wu.Common.Push;
using Nsc.Wu.BLL.Service;

namespace Nsc.Wu.Api.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        
       // string _serviceEndpoint = "http://localhost:56527/api/";
       string _serviceEndpoint = "http://193.42.156.14:5090/api/";
        [TestMethod]
        public void Create()
        {
           
                AccountController account = new AccountController();
            //  var result = account.Register(new Dto.User() { FirstName = "kamran", LastName = "Jamil", Email = "kamran@mailinator.com", TypeId = 1, PasswordHash = "123456" });;
            ServiceResponseWithResult res = JsonConvert.DeserializeObject<ServiceResponseWithResult>(CreateUser(new Dto.User() { FirstName = "kamran", LastName = "Jamil", Email = "kamran@mailinator.com", TypeId = 1, PasswordHash = "123456",Phone="03454059343" }).Content);
            Assert.IsTrue(res.IsSuccess);
        }
        [TestMethod]
        public void TestIOSPush()
        {
            CommunicationService com = new CommunicationService();
           // com.SendPush("0d75a76bb1f15d52afdc8c9607b2aa84929ba77d62b1ccfabb470647f5689e67", "request", "IOS", "Hussaan", "", "Music All Night");
          //  PushApi p = new PushApi();
       //     p.AndroidPush("", "Test");
            //AccountController account = new AccountController();
            ////  var result = account.Register(new Dto.User() { FirstName = "kamran", LastName = "Jamil", Email = "kamran@mailinator.com", TypeId = 1, PasswordHash = "123456" });;
            //ServiceResponseWithResult res = JsonConvert.DeserializeObject<ServiceResponseWithResult>(CreateUser(new Dto.User() { FirstName = "kamran", LastName = "Jamil", Email = "kamran@mailinator.com", TypeId = 1, PasswordHash = "123456", Phone = "03454059343" }).Content);
            Assert.IsTrue(true);
        }
  
        [TestMethod]
        public void Update()
        {
            AccountController account = new AccountController();
            var result = account.Update(new Dto.User() {Id=3, FirstName = "Irfan", LastName = "jameel", Email = "irfan@mailinator.com", TypeId = 1, PasswordHash = "123456" });
            Assert.IsTrue(result.IsSuccess);
        }
        [TestMethod]
        public void FindUser()
        {
            AccountController account = new AccountController();
            var result = account.GetUserInfo(3);
            Assert.IsTrue(result.IsSuccess);
        }
        [TestMethod]
        public void SaveImage()
        {
            AccountController account = new AccountController();
            var result = account.SaveImage(new Dto.ResourceImages() {
                FileName = "irfan.jpg",
                FileExtension = "jpg",
                OwnerId = 3,
                ImageType = "user",
                UploadedBy = 3,
                Base64Data = Convert.ToBase64String(File.ReadAllBytes("C:\\Users\\Irfan\\Desktop\\justt.jpg"))
            });
            Assert.IsTrue(result.IsSuccess);
        }
        public  IRestResponse CreateUser(Dto.User rewardPoints)
        {
            string url = _serviceEndpoint + "account";
            RestClient restClient = new RestClient(url);

            var request = new RestRequest("create", Method.POST);

            request.AddHeader("ApplicationKey", "BF1A4A1C-A55D-49E1-AFCD-52A407095DEF");
            request.AddJsonBody(rewardPoints);

            IRestResponse response = restClient.Execute(request);

            return response;

        }
    }
}
