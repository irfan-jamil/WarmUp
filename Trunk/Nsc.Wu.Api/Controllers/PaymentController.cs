using Nsc.Wu.Api.Filters;
using Nsc.Wu.BLL.Service;
using Nsc.Wu.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nsc.Wu.Api.Controllers
{
    [RoutePrefix("api/Payment")]
    public class PaymentController : ApiController
    {
        [Route("Pay")]
        [HttpPost]
        [AuthorizeRequest]
        public ServiceResponseWithResult Pay(Dto.PaymentDto payment)
        {
            string msg = Validate(payment);
            if (!String.IsNullOrEmpty(msg))
                return new ServiceResponseWithResult(false, msg, null);

  
            return new PaymentService().Save(payment);
        }
        [Route("Detail/{eventid}")]
        [HttpGet]
        [AuthorizeRequest]
        public ServiceResponseWithResult Detail(int eventid)
        {

            return new PaymentService().Detail(eventid);
        }
        private string Validate(Dto.PaymentDto model)
        {
           
            if (model.BeneficiaryId<=0)
            {
                return "BeneficiaryId is required";
            }
            if (model.EventId <= 0)
            {
                return "EventId is required";
            }
            if (model.PayerId <= 0)
            {
                return "PayerId is required";
            }
            if (model.Amount<=0)
            {
                return "Amount is required";
            }
            
            //if (model.TypeId == null || model.TypeId <= 0 || model.TypeId > 2)
            //{
            //    return "TypeId can be 1 0r 2";
            //}
            return "";
        }
    }
}
