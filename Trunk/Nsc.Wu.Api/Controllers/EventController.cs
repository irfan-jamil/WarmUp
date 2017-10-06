using Nsc.Wu.Api.Filters;
using Nsc.Wu.BLL.Service;
using Nsc.Wu.Common;
using Nsc.Wu.Common.Push;
using Nsc.Wu.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nsc.Wu.Api.Controllers
{
    // [Authorize]
    [RoutePrefix("api/Event")]
    public class EventController : ApiController
    {
        [Route("create")]
        [AuthorizeRequest]
        public ServiceResponseWithResult Create(Dto.Event userEvent)
        {
            string msg = Validate(userEvent);
            if (!String.IsNullOrEmpty(msg))
                return new ServiceResponseWithResult(false, msg, null);

            if (!new EventService().HaveEvent((int)userEvent.CreatedBy))
                return new ServiceResponseWithResult(false, "User already have an active event", null);
            return new EventService().Create(userEvent);
        }

        [Route("testPush")]
        [AuthorizeRequest]
        public ServiceResponseWithResult testPush(Dto.PushTest test)
        {

            CommunicationService push = new CommunicationService();
            push.SendPush(test.deviceid,0, test.type, test.platform, "TestUser", "", "TestEvent");
            return new ServiceResponseWithResult(true, "success", test);

        }
        [Route("MarkAsRead")]
        [AuthorizeRequest]
        public ServiceResponseWithResult MarkAsRead(Dto.NotificationDetailDto test)
        {

            CommunicationService push = new CommunicationService();
            return push.MarkNotificationAsRead(test.id);
        

        }
        [HttpGet]
        [Route("usernotification/{userid}/{offset}/{size}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult UserNotification(int userid, int offset, int size)
        {
            NotificationDto dto = new NotificationDto();
            dto.alert = new List<NotificationDetailDto>();
            dto.alert = new CommunicationService().UserNotifications(userid, offset, size);
            return new ServiceResponseWithResult(true, "success", dto);
        }
        [Route("eventoperation")]
        [AuthorizeRequest]
        public ServiceResponse EventOperations(Dto.EventRequestDTO dto)
        {
            return new EventService().EventOperation(dto);
        }
        [Route("Rateuserevent")]
        [AuthorizeRequest]
        public ServiceResponse RateUserEvent(Dto.UserRatingDto dto)
        {
            return new EventService().RateUserEvent(dto);
        }
        [HttpGet]
        [Route("events/{page}/{size}/{orderby}/{dir}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult Events(int page, int size, string orderby, string dir)
        {

            return new EventService().AllEvents(page, size, orderby, dir);
        }
        [HttpGet]
        [Route("userevents/{userid}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult Events(int userid)
        {

            return new EventService().AllUserEvents(userid);
        }
       
        [HttpGet]
        [Route("requests/{userid}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult Requests(int userid)
        {

            return new EventService().Requests(userid);
        }

        [HttpGet]
        [Route("Invites/{userid}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult Invites(int userid)
        {

            return new EventService().Invites(userid);
        }

        [HttpGet]
        [Route("IAmIn/{userid}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult IAmIn(int userid)
        {

            return new EventService().IamIn(userid);
        }

        [HttpGet]
        [Route("PendingRating/{userid}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult PendingRating(int userid)
        {

            return new EventService().PendingRating(userid);
        }
        [HttpGet]
        [Route("eventsLoc/{Lat}/{Lng}/{userid}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult Events(double? Lat, double? Lng, int? userid)
        {

            return new EventService().AllEvents((double)Lat, (double)Lng, (int)userid);
        }
        private string Validate(Dto.Event model)
        {
            File.AppendAllText("C://logs/temp/my.txt", "Success with" + "\r\n" + "participant=" + model.MaxParticipants + ",Created By=" + model.CreatedBy + ",Description=" + model.Description + ",Title=" + model.Title + ",Email=" + model.PaymentFee);
            File.AppendAllText("C://logs/temp/my.txt", "Success with" + "\r\n" + model.ToString());
            if (String.IsNullOrEmpty(model.Title))
            {
                return "Title is required";
            }
            if (model.EventStartDateUnix == 0)
            {
                return "Start Date is required";
            }
            if (model.EventEndDateUnix == 0)
            {
                return "End Date is required";
            }
            if (String.IsNullOrEmpty(model.PaymentFee)
              )
            {
                return "Payment Fee is required";
            }
            if (model.EventEndDateUnix == 0
              )
            {
                return "End Date is required";
            }
            if (model.MaxParticipants == null
              )
            {
                return "MaxParticipants is required";
            }
            if (model.Description == null
              )
            {
                return "Description is required";
            }
            if (model.CreatedBy == null
            )
            {
                return "CreatedBy is required";
            }

            //if (model.TypeId == null || model.TypeId <= 0 || model.TypeId > 2)
            //{
            //    return "TypeId can be 1 0r 2";
            //}
            return "";
        }
    }
}
