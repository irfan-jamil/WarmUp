using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Configuration;

//using System.Web.Http.Cors;

namespace Nsc.Wu.Api.Filters
{
 

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRequestAttribute : AuthorizationFilterAttribute
    {
        

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string token = string.Empty;

            if (!actionContext.Request.Headers.Contains("applicationKey"))
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("applicationKey is missing in request header")
                };
                return;

            }

            token = actionContext.Request.Headers.GetValues("applicationKey").First();
           
            
           
            if(!ConfigurationManager.AppSettings["AppKey"].ToString().Equals(token))
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("applicationKey is missing in request header")
                };
                return;

            }
        }
       
    }
}