using AutoMapper;
using Nsc.Wu.BLL.Mapper;
using Nsc.Wu.Common;
using Nsc.Wu.Common.Push;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Device.Location;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.BLL.Service
{
    public class EventService
    {
        public ServiceResponseWithResult Create(Dto.Event userEvent)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dto.Event, Model.Event>();
                cfg.CreateMap<Dto.EventLable, Model.EventLable>();
                cfg.CreateMap<Dto.EventLocation, Model.EventLocation>();
                cfg.CreateMap<Dto.EventParticipant, Model.EventParticipant>();
                cfg.CreateMap<Dto.EventRequest, Model.EventRequest>();

            });
            Model.Event userEventModel = AutoMapperHelper<Dto.Event, Model.Event>.MapModel(userEvent, config);
            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                //if(StringuserEventModel.PaymentFee)
                userEventModel.EventEndDate = DateTime.UtcNow.ToDateTime(userEvent.EventEndDateUnix);
                userEventModel.EventStartDate = DateTime.UtcNow.ToDateTime(userEvent.EventStartDateUnix);

                context.Events.Add(userEventModel);
                context.SaveChanges();
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "EventNotCreated", null)
                {
                    IsSuccess = false,
                    Message = "EventNotCreated,Exception=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "success", new Dto.Event()
            {
                Address = userEventModel.Address,
                Code = userEventModel.Code,
                CreatedBy = (int)userEventModel.CreatedBy,
                CreatedUtcDate = userEventModel.CreatedUtcDate,
                Description = userEventModel.Description,
                Duration = userEventModel.Duration,
                EventEndDateUnix = ((DateTime)userEventModel.EventEndDate).ToUnixTimestamp(),
                EventStartDateUnix = ((DateTime)userEventModel.EventStartDate).ToUnixTimestamp(),
                PaymentFee = userEventModel.PaymentFee,
                IsFree = (bool)userEventModel.IsFree,
                Title = userEventModel.Title,
                MaxParticipants = (int)userEventModel.MaxParticipants,
                Id = userEventModel.Id,
                EventLables = userEventModel.EventLables.Select(lab => new Dto.EventLable() { Id = lab.Id, Label = lab.Label }).ToList(),
                EventLocations = userEventModel.EventLocations.Select(loc => new Dto.EventLocation() { Id = loc.Id, Lat = loc.Lat, Lng = loc.Lng, MarkerColor = loc.MarkerColor }).ToList()
            });
        }
        public ServiceResponseWithResult AllEvents(int page,int size,string orderby,string dir)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.Event, Dto.Event>();
                cfg.CreateMap<Model.EventLable, Dto.EventLable>();
                cfg.CreateMap<Model.EventLocation, Dto.EventLocation>();
                cfg.CreateMap<Model.EventParticipant, Dto.EventParticipant>();
                cfg.CreateMap<Model.EventRequest, Dto.EventRequest>();

            });

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                List<Model.Event> userEventModel = new List<Model.Event>();
                if (String.IsNullOrEmpty(orderby) || String.IsNullOrEmpty(dir))
                {
                   userEventModel  = context.Events.OrderByDescending(ev => ev.CreatedUtcDate).Skip((page - 1) * size).Take(size).ToList();
                }
                else
                {
                    userEventModel = context.Events.OrderBy(orderby+" "+dir).Skip((page - 1) * size).Take(size).ToList();
                }
                List<Dto.Event> events = GetEventDto(userEventModel, context);
                //    AutoMapperHelper<List<Model.Event>,List<Dto.Event>>.MapList(config);
                //IMapper mapper = config.CreateMapper();
                //List<Dto.Event> events = mapper.Map<List<Model.Event>, List<Dto.Event>>(userEventModel);
                return new ServiceResponseWithResult(true, "success", events);
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }
        public ServiceResponseWithResult PendingRating(int userId)
        {

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                DateTime today = DateTime.UtcNow;

                List<Model.Event> temp = context.EventParticipants.Where(ev => ev.ParticipantId == userId
                // && DbFunctions.TruncateTime(ev.Event.EventEndDate) < today.Date
                && ev.Event.CreatedBy != userId
                && ev.Event.UserRatings.Any(rt => rt.FromUserId == userId) == false).Select(e => e.Event).ToList();
                List<Model.Event> userEventModel = temp != null ? temp.OrderByDescending(o => o.CreatedUtcDate).ToList() : new List<Model.Event>();

                return new ServiceResponseWithResult(true, "success", GetEventDto(userEventModel, context));
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }
        public ServiceResponseWithResult AllUserEvents(int userId)
        {

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Model.Event, Dto.Event>();
            //    cfg.CreateMap<Model.EventLable, Dto.EventLable>();
            //    cfg.CreateMap<Model.EventLocation, Dto.EventLocation>();
            //    cfg.CreateMap<Model.EventParticipant, Dto.EventParticipant>();
            //    cfg.CreateMap<Model.EventRequest, Dto.EventRequest>();

            //});

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                DateTime today = DateTime.UtcNow;

                List<Model.Event> temp = context.Events.Where(ev => ev.CreatedBy == userId && DbFunctions.TruncateTime(ev.EventEndDate) >= today.Date).ToList();
                List<Model.Event> userEventModel = temp != null ? temp.OrderByDescending(o => o.CreatedUtcDate).ToList() : new List<Model.Event>();
                //    AutoMapperHelper<List<Model.Event>,List<Dto.Event>>.MapList(config);
                //IMapper mapper = config.CreateMapper();
                //List<Dto.Event> events = mapper.Map<List<Model.Event>, List<Dto.Event>>(userEventModel);
                return new ServiceResponseWithResult(true, "success", GetEventDto(userEventModel, context));
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }

        public ServiceResponseWithResult IamIn(int userId)
        {
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Model.Event, Dto.Event>();
            //    cfg.CreateMap<Model.EventLable, Dto.EventLable>();
            //    cfg.CreateMap<Model.EventLocation, Dto.EventLocation>();
            //    cfg.CreateMap<Model.EventParticipant, Dto.EventParticipant>();
            //    cfg.CreateMap<Model.EventRequest, Dto.EventRequest>();

            //});

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                DateTime today = DateTime.UtcNow;

                List<Model.Event> temp = context.EventParticipants.Where(ev => ev.ParticipantId == userId && DbFunctions.TruncateTime(ev.Event.EventEndDate) >= today.Date).Select(e => e.Event).ToList();
                List<Model.Event> userEventModel = temp != null ? temp.OrderByDescending(o => o.CreatedUtcDate).ToList() : new List<Model.Event>();
                //    AutoMapperHelper<List<Model.Event>,List<Dto.Event>>.MapList(config);
                //IMapper mapper = config.CreateMapper();
                //List<Dto.Event> events = mapper.Map<List<Model.Event>, List<Dto.Event>>(userEventModel);
                return new ServiceResponseWithResult(true, "success", GetEventDto(userEventModel, context));
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }

        public ServiceResponseWithResult Requests(int userId)
        {
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Model.Event, Dto.Event>();
            //    cfg.CreateMap<Model.EventLable, Dto.EventLable>();
            //    cfg.CreateMap<Model.EventLocation, Dto.EventLocation>();
            //    cfg.CreateMap<Model.EventParticipant, Dto.EventParticipant>();
            //    cfg.CreateMap<Model.EventRequest, Dto.EventRequest>();

            //});

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                DateTime today = DateTime.UtcNow;

                List<Model.Event> temp = context.EventRequests.Where(ev => ev.SentTo == userId && ev.Type == "request" && DbFunctions.TruncateTime(ev.Event.EventEndDate) >= today.Date && ev.Status == "pending").Select(e => e.Event).ToList();
                List<Model.Event> userEventModel = temp != null ? temp.OrderByDescending(o => o.CreatedUtcDate).ToList() : new List<Model.Event>();
                //    AutoMapperHelper<List<Model.Event>,List<Dto.Event>>.MapList(config);
                //IMapper mapper = config.CreateMapper();
                //List<Dto.Event> events = mapper.Map<List<Model.Event>, List<Dto.Event>>(userEventModel);
                return new ServiceResponseWithResult(true, "success", GetEventDto(userEventModel, context));
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }

        public ServiceResponseWithResult Invites(int userId)
        {
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Model.Event, Dto.Event>();
            //    cfg.CreateMap<Model.EventLable, Dto.EventLable>();
            //    cfg.CreateMap<Model.EventLocation, Dto.EventLocation>();
            //    cfg.CreateMap<Model.EventParticipant, Dto.EventParticipant>();
            //    cfg.CreateMap<Model.EventRequest, Dto.EventRequest>();

            //});

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                DateTime today = DateTime.UtcNow;

                List<Model.Event> temp = context.EventRequests.Where(ev => ev.SentTo == userId && ev.Type == "invite" && DbFunctions.TruncateTime(ev.Event.EventEndDate) >= today.Date && ev.Status == "pending").Select(e => e.Event).ToList();
                List<Model.Event> userEventModel = temp != null ? temp.OrderByDescending(o => o.CreatedUtcDate).ToList() : new List<Model.Event>();
                //    AutoMapperHelper<List<Model.Event>,List<Dto.Event>>.MapList(config);
                //IMapper mapper = config.CreateMapper();
                //List<Dto.Event> events = mapper.Map<List<Model.Event>, List<Dto.Event>>(userEventModel);
                return new ServiceResponseWithResult(true, "success", GetEventDto(userEventModel, context));
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }
        public ServiceResponse EventOperation(Dto.EventRequestDTO dto)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            Model.Event eve = context.Events.FirstOrDefault(p => p.Id == dto.EventId);
          //  PushApi push = new PushApi();

           

            bool isSuccess = false;
            if (eve == null)
                return new ServiceResponse()
                {

                    IsSuccess = false,
                    Message = "EventNotFound"
                };
            if (dto.Action.Equals("invite"))
            {
                isSuccess = AddInvite(context, dto, eve);
                

            }
            else if (dto.Action.Equals("request"))
            {
                isSuccess = AddRequest(context, dto, eve);
            }
            else if (dto.Action.Equals("AcceptRequest"))
            {
                isSuccess = AcceptRequest(context, dto, eve);
            }
            else if (dto.Action.Equals("AcceptInvite"))
            {
                isSuccess = AcceptInvite(context, dto, eve);
            }
            else if (dto.Action.Equals("RejectRequest"))
            {
                isSuccess = RejectRequest(context, dto, eve);
            }
            else if (dto.Action.Equals("RejectInvite"))
            {
                isSuccess = RejectRequest(context, dto, eve);
            }
            if (!isSuccess)
            {
                return new ServiceResponse()
                {

                    IsSuccess = false,
                    Message = "AlreadyAdded"
                };
            }
            return new ServiceResponse()
            {

                IsSuccess = true,
                Message = "success"
            };
        }

        public ServiceResponse RateUserEvent(Dto.UserRatingDto dto)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            Model.Event eve = context.Events.FirstOrDefault(p => p.Id == dto.EventId);

            context.UserRatings.Add(new Model.UserRating() { Comment = dto.Comment, EventId = eve.Id, FromUserId = dto.userid, Rating = dto.Rating });
            int res = context.SaveChanges();
            if (res <= 0)
            {
                return new ServiceResponse()
                {

                    IsSuccess = false,
                    Message = "failure"
                };
            }
            return new ServiceResponse()
            {

                IsSuccess = true,
                Message = "success"
            };
        }
        public ServiceResponseWithResult AllEvents(double Lat, double Lng, int userid)
        {
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Model.Event, Dto.Event>();
            //    cfg.CreateMap<Model.EventLable, Dto.EventLable>();
            //    cfg.CreateMap<Model.EventLocation, Dto.EventLocation>();
            //    //cfg.CreateMap<Model.EventParticipant, Dto.EventParticipant>();
            //    //cfg.CreateMap<Model.EventRequest, Dto.EventRequest>();

            //});

            try
            {
                Model.WarmUpEntities context = new Model.WarmUpEntities();
                List<Model.Event> userEventModel = GetNearByEvents(Lat, Lng, userid);
                //    AutoMapperHelper<List<Model.Event>,List<Dto.Event>>.MapList(config);
                //IMapper mapper = config.CreateMapper();
                //List<Dto.Event> events = mapper.Map<List<Model.Event>, List<Dto.Event>>(userEventModel);
                return new ServiceResponseWithResult(true, "success", GetEventDto(userEventModel, context));
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Internal Error", null)
                {
                    IsSuccess = false,
                    Message = "Internal Error=" + err.StackTrace.ToString(),

                };
            }
            return new ServiceResponseWithResult(true, "fail", null);
        }

        public bool HaveEvent(int userid)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            Model.Event userevent = context.Events.Where(ev => DbFunctions.TruncateTime(ev.EventEndDate.Value) >= DbFunctions.TruncateTime(DateTime.UtcNow) && ev.CreatedBy == userid).FirstOrDefault();
            if (userevent != null)
            {
                return false;
            }
            return true;
        }
        private List<Model.Event> GetNearByEvents(double sLat, double sLng, int userid)
        {
            Model.WarmUpEntities context = new Model.WarmUpEntities();
            DateTime today = DateTime.UtcNow;
            List<Model.Event> userEventModel = context.Events.Where(e => DbFunctions.TruncateTime(e.EventEndDate) >= today.Date).ToList();
            List<Model.Event> resultEvents = new List<Model.Event>();
            double minDistance = Convert.ToDouble(ConfigurationManager.AppSettings["MinDistance"].ToString());
            foreach (var eve in userEventModel)
            {
                Model.EventLocation loc = eve.EventLocations.FirstOrDefault();
                if (loc != null &&
                    !String.IsNullOrEmpty(loc.Lat) &&
                    !String.IsNullOrEmpty(loc.Lng) &&
                    GetDistance(sLat, sLng, Convert.ToDouble(loc.Lat), Convert.ToDouble(loc.Lng)) < minDistance)
                {
                    resultEvents.Add(eve);
                }
            }
            return resultEvents;
        }
        private double GetDistance(double sLat, double sLng, double eLat, double eLng)
        {
            var sCoord = new GeoCoordinate(sLat, sLng);
            var eCoord = new GeoCoordinate(eLat, eLng);
            double dist = sCoord.GetDistanceTo(eCoord);
            return dist;
        }
        private List<Dto.Event> GetEventDto(List<Model.Event> list, Model.WarmUpEntities context)
        {
            DateTime today = DateTime.UtcNow;
            List<Dto.Event> events = list.Select(e => new Dto.Event()
            {
                Address = e.Address,
                CreatedBy = e.CreatedBy != null ? (int)e.CreatedBy : 0,
                Code = e.Code,
                CreatedUtcDate = e.CreatedUtcDate,
                Description = e.Description,
                Duration = e.Duration,
                IsFree = e.IsFree != null ? (bool)e.IsFree : false,
                Title = e.Title,
                PaymentFee = e.PaymentFee,
                UpdatedUtcDate = e.UpdatedUtcDate,
                Id = e.Id,
                EventStartDateUnix = e.EventStartDate == null ? 0 : ((DateTime)e.EventStartDate).ToUnixTimestamp(),
                EventEndDateUnix = e.EventStartDate == null ? 0 : ((DateTime)e.EventEndDate).ToUnixTimestamp(),
                MaxParticipants = (int)e.MaxParticipants,
                Creater = new Dto.UserProfleDto()
                {
                    FirstName = e.UserAccount.FirstName,
                    LastName = e.UserAccount.LastName,
                    Email = e.UserAccount.Email,
                    Phone = e.UserAccount.Phone,
                    UserName = e.UserAccount.UserName,
                    TypeId = e.UserAccount.TypeId,
                    Gender = e.UserAccount.Gender,
                    About = e.UserAccount.About,
                    Address = e.UserAccount.Address,
                    DobUnix = e.UserAccount.Dob == null ? null : ((DateTime)e.UserAccount.Dob).ToUnixTimestamp().ToString(),
                    Rating = e.UserAccount.Rating,
                    ProfilePic = e.UserAccount?.ResourceImages?.FirstOrDefault(o => o.ImageType == "user" && o.IsProfilePic == true) != null ? e.UserAccount?.ResourceImages?.FirstOrDefault(o => o.ImageType == "user" && o.IsProfilePic == true).FileURL : "",

                },
                EventLocations = e.EventLocations.Select(loc => new Dto.EventLocation()
                {
                    Address = loc.Address,
                    EventId = loc.EventId,
                    Lat = loc.Lat,
                    Lng = loc.Lng,
                    MarkerColor = loc.MarkerColor
                }).ToList(),
                EventLables = e.EventLables.Select(lab => new Dto.EventLable()
                {
                    IsActive = lab.IsActive,
                    Label = lab.Label
                }).ToList(),
                EventParticipants = e.EventParticipants.Select(p => new Dto.UserProfleDto()
                {
                    FirstName = p.UserAccount.FirstName,
                    Address = p.UserAccount.Address,
                    CreatedUtcDate = p.UserAccount.CreatedUtcDate,
                    Email = p.UserAccount.Email,
                    Id = p.UserAccount.Id,
                    IsActive = p.UserAccount.IsActive,
                    LastName = p.UserAccount.LastName,
                    Phone = p.UserAccount.Phone,
                    ProfilePic = p.UserAccount?.ResourceImages?.FirstOrDefault(o => o.ImageType == "user") != null ? p.UserAccount.ResourceImages?.FirstOrDefault(o => o.ImageType == "user").FileURL : "",
                    TypeId = p.UserAccount.TypeId,
                    UpdatedUtcDate = p.UserAccount.UpdatedUtcDate,
                    UserName = p.UserAccount.UserName,
                    Gender = p.UserAccount.Gender,
                    About = p.UserAccount.About,
                    DobUnix = p.UserAccount.Dob == null ? null : ((DateTime)e.UserAccount.Dob).ToUnixTimestamp().ToString(),
                    Rating = p.UserAccount.Rating,
                    Payment = p.UserAccount.Payments.Select(pay => new Dto.PaymentDto()
                    {
                        Amount = pay.Amount,
                        BarCode = pay.BarCode,
                        BeneficiaryId = pay.BeneficiaryId,
                        Currency = pay.Currency,
                        DeductionAmont = pay.DeductionAmont,
                        EventEntryCode = pay.EventEntryCode,
                        EventId = pay.EventId,
                        Id = pay.Id,
                        PayerId = pay.PayerId,
                        PaymentDate = pay.PaymentDate,
                        PaymentMethod = pay.PaymentMethod,
                        TransactionId = pay.TransactionId,
                        TransferAmount = pay.TransferAmount
                    }).FirstOrDefault()

                }).ToList(),
                EventRequests = e.EventRequests.Where(req => req.Status == "pending").Select(p => new Dto.UserProfleDto()
                {

                    FirstName = p.Type.Equals("invite") ? p.UserAccount1.FirstName : p.UserAccount.FirstName,
                    Address = p.Type.Equals("invite") ? p.UserAccount1.Address : p.UserAccount.Address,
                    CreatedUtcDate = p.Type.Equals("invite") ? p.UserAccount1.CreatedUtcDate : p.UserAccount.CreatedUtcDate,
                    Email = p.Type.Equals("invite") ? p.UserAccount1.Email : p.UserAccount.Email,
                    Id = p.Type.Equals("invite") ? p.UserAccount1.Id : p.UserAccount.Id,
                    IsActive = p.Type.Equals("invite") ? p.UserAccount1.IsActive : p.UserAccount.IsActive,
                    LastName = p.Type.Equals("invite") ? p.UserAccount1.LastName : p.UserAccount.LastName,
                    Phone = p.Type.Equals("invite") ? p.UserAccount1.Phone : p.UserAccount.Phone,
                    ProfilePic = p.Type.Equals("invite") ? (p.UserAccount1.ResourceImages.FirstOrDefault(o => o.ImageType == "user") != null ? p.UserAccount1.ResourceImages.FirstOrDefault(o => o.ImageType == "user").FileURL : "") :
                      (p.UserAccount.ResourceImages.FirstOrDefault(o => o.ImageType == "user") != null ? p.UserAccount.ResourceImages.FirstOrDefault(o => o.ImageType == "user").FileURL : ""),
                    TypeId = p.Type.Equals("invite") ? p.UserAccount1.TypeId : p.UserAccount.TypeId,
                    UpdatedUtcDate = p.Type.Equals("invite") ? p.UserAccount1.UpdatedUtcDate : p.UserAccount.UpdatedUtcDate,
                    UserName = p.Type.Equals("invite") ? p.UserAccount1.UserName : p.UserAccount.UserName,
                    Gender = p.Type.Equals("invite") ? p.UserAccount1.Gender : p.UserAccount.Gender,
                    About = p.Type.Equals("invite") ? p.UserAccount1.About : p.UserAccount.About,
                    DobUnix = p.Type.Equals("invite") ? (p.UserAccount1.Dob == null ? null : ((DateTime)p.UserAccount1.Dob).ToUnixTimestamp().ToString()) : (p.UserAccount.Dob == null ? null : ((DateTime)e.UserAccount.Dob).ToUnixTimestamp().ToString()),
                    Rating = p.Type.Equals("invite") ? p.UserAccount1.Rating : p.UserAccount.Rating,
                    Type = p.Type
                }).ToList(),
                EventImages = context.ResourceImages.Where(o => o.ImageType == "event" && o.OwnerId == e.Id).Select(o => o.FileURL).ToList()

            }).ToList();
            return events;
        }
        private bool AddInvite(Model.WarmUpEntities context, Dto.EventRequestDTO dto, Model.Event eve)
        {
            Model.EventRequest req = context.EventRequests.FirstOrDefault(o => o.EventId == eve.Id && o.SentTo == dto.UserId && o.Type == "invite");
          
            if (req != null)
                return true;
            eve.EventRequests.Add(new Model.EventRequest()
            {
                EventId = dto.EventId,
                CreatedUtcDate = DateTime.UtcNow,
                SentBy = eve.CreatedBy,
                SentTo = dto.UserId,
                Type = "invite",
                Status = "pending",


            });

         //   Model.UserAccount receiver = context.UserAccounts.FirstOrDefault(u=>u.Id==dto.UserId);
            context.SaveChanges();
            CommunicationService com = new CommunicationService();
            string userId = dto.UserId.ToString();
            Model.DevicePush deviceId = context.DevicePushes.FirstOrDefault(u => u.ClientId == userId);
            if(deviceId!=null)
            com.SendPush(deviceId.PushId, Convert.ToInt32(deviceId.ClientId),"invite", deviceId.Platform, eve.UserAccount.FirstName, "", eve.Title);
            return true;
        }

        private bool AddRequest(Model.WarmUpEntities context, Dto.EventRequestDTO dto, Model.Event eve)
        {
            if (eve.EventRequests.FirstOrDefault(o => o.EventId == eve.Id && o.SentBy == dto.UserId && o.Type == "request") != null)
            {
                return false;
            }
            eve.EventRequests.Add(new Model.EventRequest()
            {
                EventId = dto.EventId,
                CreatedUtcDate = DateTime.UtcNow,
                SentBy = dto.UserId,
                SentTo = eve.CreatedBy,
                Type = "request",
                Status = "pending",


            });
            context.SaveChanges();

            CommunicationService com = new CommunicationService();
            string userId = eve.CreatedBy.ToString();
            Model.UserAccount sender = context.UserAccounts.FirstOrDefault(u => u.Id == dto.UserId);
            Model.DevicePush deviceId = context.DevicePushes.OrderByDescending(o=>o.CreatedUTCDateTime).FirstOrDefault(u => u.ClientId == userId);
            if (deviceId != null)
                com.SendPush(deviceId.PushId, Convert.ToInt32(deviceId.ClientId), "request", deviceId.Platform, sender.FirstName, "", eve.Title);
          
            return true;
        }

        private bool AcceptRequest(Model.WarmUpEntities context, Dto.EventRequestDTO dto, Model.Event eve)
        {
            Model.EventRequest req = context.EventRequests.FirstOrDefault(o => o.EventId == eve.Id && o.SentBy == dto.UserId);

            if (req == null)
                return false;
            req.Status = "Accpeted";

            eve.EventParticipants.Add(new Model.EventParticipant()
            {
                EventId = dto.EventId,
                CreatedUtcDate = DateTime.UtcNow,
                ParticipantId = dto.UserId,
                IsActive = true,

            });
            context.SaveChanges();
            Model.UserAccount receiver = context.UserAccounts.FirstOrDefault(u=>u.Id==dto.UserId);
            CommunicationService com = new CommunicationService();
            string userId = dto.UserId.ToString();
            Model.DevicePush deviceId = context.DevicePushes.FirstOrDefault(u => u.ClientId == userId);
            if (deviceId != null)
                com.SendPush(deviceId.PushId, Convert.ToInt32(deviceId.ClientId), "acceptrequest", deviceId.Platform, eve.UserAccount.FirstName, "", eve.Title);
           // context.SaveChanges();
            
            return true;
        }

        private bool AcceptInvite(Model.WarmUpEntities context, Dto.EventRequestDTO dto, Model.Event eve)
        {
            Model.EventRequest req = context.EventRequests.FirstOrDefault(o => o.EventId == eve.Id && o.SentTo == dto.UserId && o.Type == "invite");

            if (req == null)
                return false;
            req.Status = "Accpeted";

            eve.EventParticipants.Add(new Model.EventParticipant()
            {
                EventId = dto.EventId,
                CreatedUtcDate = DateTime.UtcNow,
                ParticipantId = dto.UserId,
                IsActive = true,

            });
            context.SaveChanges();

            Model.UserAccount receiver = context.UserAccounts.FirstOrDefault(u => u.Id == dto.UserId);
            CommunicationService com = new CommunicationService();
            string userId = eve.CreatedBy.ToString();
            Model.DevicePush deviceId = context.DevicePushes.FirstOrDefault(u => u.ClientId == userId);
            if (deviceId != null)
                com.SendPush(deviceId.PushId, Convert.ToInt32(deviceId.ClientId),"acceptinvite", deviceId.Platform, receiver.FirstName, "", eve.Title);
            return true;
        }

        private bool RejectInvite(Model.WarmUpEntities context, Dto.EventRequestDTO dto, Model.Event eve)
        {
            Model.EventRequest req = context.EventRequests.FirstOrDefault(o => o.EventId == eve.Id && o.SentTo == dto.UserId && o.Type == "invite");

            if (req == null)
                return false;
            req.Status = "Rejected";
           // context.EventRequests.Remove(req);
            context.SaveChanges();
            return true;
        }
        private bool RejectRequest(Model.WarmUpEntities context, Dto.EventRequestDTO dto, Model.Event eve)
        {
            Model.EventRequest req = context.EventRequests.FirstOrDefault(o => o.EventId == eve.Id && o.SentBy == dto.UserId);

            if (req == null)
                return false;
          //  req.Status = "Rejected";
            context.EventRequests.Remove(req);
            //eve.EventParticipants.Add(new Model.EventParticipant()
            //{
            //    EventId = dto.EventId,
            //    CreatedUtcDate = DateTime.UtcNow,
            //    ParticipantId = dto.UserId,
            //    IsActive = true,

            //});
            context.SaveChanges();
            return true;
        }
    }
}
