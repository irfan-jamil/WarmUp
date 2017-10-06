using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nsc.Wu.AdminApp.Models;
using Nsc.Wu.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Nsc.Wu.AdminApp.Controllers
{
    public class EventManagerController : Controller
    {
        string _serviceEndpoint = "http://localhost:56527/api/";
        // GET: EventManager
        public ActionResult Index()
        {
            return View();
        }

        public IRestResponse CreateEvent(Nsc.Wu.Dto.Event events)
        {
            string url = _serviceEndpoint + "account";
            RestClient restClient = new RestClient(url);

            var request = new RestRequest("create", Method.POST);

            request.AddHeader("ApplicationKey", "BF1A4A1C-A55D-49E1-AFCD-52A407095DEF");
            request.AddJsonBody(events);

            IRestResponse response = restClient.Execute(request);

            return response;

        }

        public IRestResponse AllEvents(int page, int size, string orderby, string dir)
        {
            string url = _serviceEndpoint + "event";
            RestClient restClient = new RestClient(url);

            var request = new RestRequest("events/"+page+"/"+size+"/"+orderby+"/"+dir, Method.GET);

            request.AddHeader("ApplicationKey", "BF1A4A1C-A55D-49E1-AFCD-52A407095DEF");

            IRestResponse response = restClient.Execute(request);

            return response;

        }
        private static JsonResult _json = new JsonResult
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            ContentType = "application/json; charset=utf-8",
            ContentEncoding = Encoding.UTF8
        };
        public  JsonResult GetData(DTSearchParam dto)
        {
            if (dto.Start == 0)
                dto.Start = 1;

            ServiceResponseWithResult res = JsonConvert.DeserializeObject<ServiceResponseWithResult>(AllEvents(dto.Start,dto.Length,dto.OrderBy,dto.Dir).Content);
            //_json.Data = new
            //{
            //    success = "true",
            //    code = 200,
            //    message = "success",
            //    data = new JavaScriptSerializer().Serialize(res.Result)
            //};

            var dataTable = new JObject() as dynamic;

            DTResponse resp = new DTResponse();
            List<Dto.Event> events = JsonConvert.DeserializeObject<List<Nsc.Wu.Dto.Event>>(res.Result.ToString());
            resp.draw = dto.Draw.ToString();
            resp.length = dto.Length.ToString();
            resp.recordsFiltered = events.Count.ToString();
            resp.recordsTotal = events.Count.ToString();
            resp.data = events.Select(sec => new EventDto()
            {
                Description = sec.Description,
                Id = sec.Id.ToString(),
                Title = sec.Title
            }).ToList();
            //dataTable.recordsTotal = events.Count();
            //dataTable.recordsFiltered = events.Count();
            //dataTable.length = dto.Length;
            //dataTable.draw = dto.Draw;
            //dataTable.data = new JavaScriptSerializer().Serialize(events.Select(sec=>new EventDto() {
            //    Description = sec.Description,
            //    Id = sec.Id.ToString(),
            //    Title = sec.Title
            //}).ToList());
            return Json(resp, JsonRequestBehavior.AllowGet);
          //  return dataTable.ToString();
        }
    }
}