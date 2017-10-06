using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nsc.Wu.Api;
using Nsc.Wu.Api.Controllers;
using Stripe;

namespace Nsc.Wu.Api.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            IEnumerable<string> result = controller.Get();
            StripeConfiguration.SetApiKey("sk_test_ncF6JRdb48RaKZpJjSHuckBQ");

            var customerOptions = new StripeCustomerCreateOptions()
            {
                Description = "Customer for sophia.jones@example.com",
                Email = "irfan@mailinator.com"
               // SourceToken = "tok_visa"
            };
            
            var customerService = new StripeCustomerService();
            StripeCustomer customer = null;//customerService.Create(customerOptions);
            customer = customerService.Get("cus_BP4LUAc30Q2918");
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("value1", result.ElementAt(0));
            Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            string result = controller.Get(5);

            // Assert
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Post("value");

            // Assert
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Put(5, "value");

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Delete(5);

            // Assert
        }
    }
}
