using AutoMapper;
using Nsc.Wu.BLL.Mapper;
using Nsc.Wu.Common;
using Nsc.Wu.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.BLL.Service
{
    public class AccountService
    {
        public ServiceResponseWithResult CreateUser(User obj)
        {

            Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {
                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    Model.UserAccount temp = context.UserAccounts.FirstOrDefault(i => i.Email == obj.Email);
                    if (temp != null)
                    {
                        return new ServiceResponseWithResult(false, "", null)
                        {
                            IsSuccess = true,
                            Message = "UserAlreadyExist",
                            Result = MapModel(temp, context)

                        };
                    }


                    newUser.IsActive = 1;
                    newUser.CreatedUtcDate = DateTime.UtcNow;
                    newUser.Dob = !String.IsNullOrEmpty(obj.DobUnix) ? DateTime.UtcNow.ToDateTime(Convert.ToInt64(obj.DobUnix)) : newUser.Dob;
                    context.UserAccounts.Add(newUser);
                    context.SaveChanges();

                    if (!String.IsNullOrEmpty(obj.DeviceId))
                    {
                        context.DevicePushes.Add(new Model.DevicePush() { ClientId = newUser.Id.ToString(), Platform = obj.Platform, State = true, CreatedUTCDateTime = DateTime.UtcNow, PushId = obj.DeviceId });
                        context.SaveChanges();
                    }
                    return new ServiceResponseWithResult(true, "", null)
                    {
                        IsSuccess = true,
                        Message = "UserCreated",
                        Result = MapModel(newUser, context)
                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "", null)
                {
                    IsSuccess = false,
                    Message = "Exception=" + err.StackTrace.ToString(),


                };
            }
        }

        public ServiceResponseWithResult AddDevice(User obj)
        {

            Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {
                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    Model.UserAccount temp = context.UserAccounts.FirstOrDefault(i => i.Email == obj.Email);
                    if (temp == null)
                    {
                        return new ServiceResponseWithResult(false, "", null)
                        {
                            IsSuccess = true,
                            Message = "UserNotExist",
                            Result = null

                        };
                    }


                    if (!String.IsNullOrEmpty(obj.DeviceId))
                    {
                        context.DevicePushes.Add(new Model.DevicePush() { ClientId = temp.Id.ToString(), Platform = obj.Platform, State = true, CreatedUTCDateTime = DateTime.UtcNow, PushId = obj.DeviceId });
                        context.SaveChanges();
                    }
                    return new ServiceResponseWithResult(true, "", null)
                    {
                        IsSuccess = true,
                        Message = "DeviceRegistered",
                        Result = MapModel(newUser, context)
                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "", null)
                {
                    IsSuccess = false,
                    Message = "Exception=" + err.StackTrace.ToString(),


                };
            }
        }
        public ServiceResponseWithResult UpdateUser(User obj)
        {

            Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {
                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    Model.UserAccount userModel = context.UserAccounts.FirstOrDefault(i => i.Id == obj.Id);
                    if (userModel == null)
                    {
                        return new ServiceResponseWithResult(false, "", null)
                        {
                            IsSuccess = false,
                            Message = "UserNotExist"
                        };
                    }
                    userModel.FirstName = newUser.FirstName;
                    userModel.LastName = newUser.LastName;
                    //  userModel.Email = newUser.Email;
                    userModel.Address = newUser.Address;
                    userModel.UserName = newUser.UserName;
                    userModel.Phone = newUser.Phone;
                    userModel.About = newUser.About;
                    //  userModel.Dob = newUser.Dob?? userModel.Dob;
                    userModel.Email = newUser.Email ?? userModel.Email;
                    userModel.UpdatedUtcDate = DateTime.UtcNow;
                    userModel.Gender = newUser.Gender ?? userModel.Gender;
                    userModel.Rating = newUser.Rating ?? userModel.Rating;
                    userModel.Dob = !String.IsNullOrEmpty(obj.DobUnix) ? DateTime.UtcNow.ToDateTime(Convert.ToInt64(obj.DobUnix)) : userModel.Dob;
                    userModel.Lat = String.IsNullOrEmpty(obj.Lat) ? userModel.Lat : obj.Lat;
                    userModel.Lng = String.IsNullOrEmpty(obj.Lng) ? userModel.Lng : obj.Lng;
                    //context.UserAccounts.(newUser);
                    context.SaveChanges();

                    //if (!String.IsNullOrEmpty(obj.DeviceId))
                    //{
                    //    context.DevicePushes.Add(new Model.DevicePush() { ClientId = newUser.Id.ToString(), Platform = obj.Platform, State = true, CreatedUTCDateTime = DateTime.UtcNow, PushId = obj.DeviceId });
                    //    context.SaveChanges();
                    //}
                    return new ServiceResponseWithResult(true, "", null)
                    {
                        IsSuccess = true,
                        Message = "UserUpdated",
                        Result = MapModel(userModel, context)
                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "", null)
                {
                    IsSuccess = false,
                    Message = "Exception=" + err.StackTrace.ToString()
                };
            }
        }

        public ServiceResponseWithResult Find(int? id)
        {

            //Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {
                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    Model.UserAccount obj = context.UserAccounts.FirstOrDefault(i => i.Id == id);
                    if (obj == null)
                    {
                        return new ServiceResponseWithResult(false, "UserNotExist", null)
                        {
                            IsSuccess = false,
                            Message = "UserNotExist",
                            Result = obj
                        };
                    }


                    return new ServiceResponseWithResult(true, "Success", obj)
                    {
                        IsSuccess = true,
                        Message = "Success",
                        Result = MapModel(obj, context)


                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "UserNotExist", null)
                {
                    IsSuccess = false,
                    Message = "UserNotExist",

                };
            }
        }
 
      
        public ServiceResponseWithResult NearByFriends(int? userid, string gender,double? minDistance, double? maxDistance, int? minAge, int? maxAge, double Lat, double Lng)
        {
          //  double minDistance = Convert.ToDouble(ConfigurationManager.AppSettings["MinDistance"].ToString());
            List<Dto.User> AllUsers = new List<Dto.User>();
            //Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {

                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    List<Model.UserAccount> users = context.UserAccounts.ToList();

                    foreach (var user in users)
                    {
                        double currentDistance = GetDistance(Lat, Lng, Convert.ToDouble(user.Lat), Convert.ToDouble(user.Lng));
                        if (user != null &&
                            user.Id != userid &&
                               !String.IsNullOrEmpty(user.Lat) &&
                               !String.IsNullOrEmpty(user.Lng) &&
                              currentDistance >= minDistance && 
                              currentDistance <= maxDistance
                             

                              )
                        {
                            if (
                                !(maxAge != 0 && user.Dob != null &&
                              (DateTime.UtcNow.Year - user.Dob.Value.Year) >= minAge &&
                              (DateTime.UtcNow.Year - user.Dob.Value.Year) <= maxAge)
                              )
                                continue;
                            if (!(gender.Equals("0")))
                            {
                                if (!(!String.IsNullOrEmpty(user.Gender) && user.Gender == gender))

                                    continue;
                            }
                                AllUsers.Add(MapModel(user, context));
                        }
                    }



                    return new ServiceResponseWithResult(true, "Success", AllUsers)
                    {
                        IsSuccess = true,
                        Message = "Success",
                        Result = AllUsers


                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "UserNotExist", null)
                {
                    IsSuccess = false,
                    Message = "UserNotExist",

                };
            }
        }
        public ServiceResponseWithResult UserImage(int? id)
        {

            //Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {
                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    Model.UserAccount obj = context.UserAccounts.FirstOrDefault(i => i.Id == id);
                    if (obj == null)
                    {
                        return new ServiceResponseWithResult(false, "UserNotExist", null)
                        {
                            IsSuccess = false,
                            Message = "UserNotExist",
                            Result = obj
                        };
                    }

                    Model.ResourceImage image = context.ResourceImages.FirstOrDefault(i => i.Id == id && i.ImageType == "user");
                    if (image == null)
                    {
                        return new ServiceResponseWithResult(false, "NoImageFound", null)
                        {
                            IsSuccess = false,
                            Message = "NoImageFound",
                            Result = null
                        };
                    }
                    return new ServiceResponseWithResult(true, "Success", obj)
                    {
                        IsSuccess = true,
                        Message = "Success",
                        Result = AutoMapperHelper<Model.ResourceImage, Dto.ResourceImages>.MapModel(image)


                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "UserNotExist", null)
                {
                    IsSuccess = false,
                    Message = "UserNotExist",


                };
            }
        }

        public ServiceResponseWithResult SaveImage(Dto.ResourceImages image)
        {

            //Model.UserAccount newUser = AutoMapperHelper<User, Model.UserAccount>.MapModel(obj);
            try
            {
                using (Model.WarmUpEntities context = new Model.WarmUpEntities())
                {
                    int ownerId = 0;
                    if (image.ImageType.Equals("user"))
                    {
                        Model.UserAccount obj = context.UserAccounts.FirstOrDefault(i => i.Id == image.OwnerId);
                        if (obj == null)
                        {
                            return new ServiceResponseWithResult(false, "UserNotExist", null)
                            {
                                IsSuccess = false,
                                Message = "UserNotExist",
                                Result = obj
                            };

                        }
                        ownerId = obj.Id;

                    }
                    else if (image.ImageType.Equals("event"))
                    {
                        Model.Event userEvent = context.Events.FirstOrDefault(e => e.Id == image.OwnerId);
                        if (userEvent == null)
                        {
                            return new ServiceResponseWithResult(false, "EventNotExist", null)
                            {
                                IsSuccess = false,
                                Message = "EventNotExist",
                                Result = null
                            };
                        }
                        ownerId = userEvent.Id;
                    }
                    else
                    {
                        return new ServiceResponseWithResult(false, "InvalidImageType", null)
                        {
                            IsSuccess = false,
                            Message = "InvalidImageType",
                            Result = null
                        };
                    }
                    Model.UserAccount obj1 = context.UserAccounts.FirstOrDefault(i => i.Id == image.UploadedBy);
                    if (obj1 == null)
                    {
                        return new ServiceResponseWithResult(false, "UploaderNotExist", null)
                        {
                            IsSuccess = false,
                            Message = "UploaderNotExist",
                            Result = null
                        };

                    }
                    Model.ResourceImage imageModel = new Model.ResourceImage();
                    imageModel.FileName = image.FileName;
                    imageModel.FileExtension = image.FileExtension;
                    imageModel.CreatedUtcDate = DateTime.UtcNow;
                    imageModel.FileDirPath = image.FileDirPath;
                    imageModel.Id = image.Id;
                    imageModel.ImageType = image.ImageType;
                    imageModel.OwnerId = image.OwnerId;
                    imageModel.UpdatedUtcDate = DateTime.UtcNow;
                    imageModel.UploadedBy = image.UploadedBy;
                    imageModel.FileURL = image.FileURL;
                    imageModel.IsProfilePic = image.IsProfilePic;
                    context.ResourceImages.Add(imageModel);
                    context.SaveChanges();
                    return new ServiceResponseWithResult(true, "Success", image)
                    {
                        IsSuccess = true,
                        Message = "Success",
                        Result = image


                    };
                }
            }
            catch (Exception err)
            {
                return new ServiceResponseWithResult(false, "Exception=" + err.StackTrace.ToString(), null)
                {
                    IsSuccess = false,
                    Message = "Exception=" + err.StackTrace.ToString()
                };
            }
        }
        private double GetDistance(double sLat, double sLng, double eLat, double eLng)
        {
            var sCoord = new GeoCoordinate(sLat, sLng);
            var eCoord = new GeoCoordinate(eLat, eLng);
            double dist = sCoord.GetDistanceTo(eCoord);
            return dist;
        }
        private User MapModel(Model.UserAccount obj, Model.WarmUpEntities context)
        {
            User temp = new User();
            temp.About = obj.About;
            temp.Address = obj.Address;
            temp.CreatedUtcDate = obj.CreatedUtcDate;
            temp.DobUnix = obj.Dob != null ? ((DateTime)obj.Dob).ToUnixTimestamp().ToString() : null;
            temp.Email = obj.Email;
            temp.FirstName = obj.FirstName;
            temp.Id = obj.Id;
            temp.IsActive = obj.IsActive;
            temp.LastName = obj.LastName;
            temp.PasswordHash = obj.PasswordHash;
            temp.Phone = obj.Phone;
            temp.TypeId = obj.TypeId;
            temp.UpdatedUtcDate = obj.UpdatedUtcDate;
            temp.UserName = obj.UserName;
            temp.Gender = obj.Gender;
            temp.UserImages = context.ResourceImages.Where(c => c.ImageType == "user" && c.OwnerId == obj.Id)?.Select(img => img.FileURL).ToList();
            temp.ProfilePic = context.ResourceImages.Where(c => c.ImageType == "user" && c.OwnerId == obj.Id && c.IsProfilePic == true)?.Select(img => img.FileURL).FirstOrDefault();
            return temp;
        }
    }
}
