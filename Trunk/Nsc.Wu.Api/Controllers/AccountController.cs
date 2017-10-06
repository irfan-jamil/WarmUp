using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Nsc.Wu.Api.Models;
using Nsc.Wu.Api.Providers;
using Nsc.Wu.Api.Results;
using Nsc.Wu.Dto;
using Nsc.Wu.BLL.Service;
using Nsc.Wu.Common;
using Nsc.Wu.Api.Filters;
using System.Configuration;
using System.IO;

namespace Nsc.Wu.Api.Controllers
{
   // [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("find/{id}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult GetUserInfo(int? id)
        {

            var result =  new AccountService().Find(id);
            result.Result = result.Result;
            return result;
        }

        [HttpGet]
        [Route("NearByFriends/{Lat}/{Lng}/{gender}/{minDistance}/{maxDistance}/{minAge}/{maxAge}/{id}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult NearByFriends(string Lat,string Lng,string gender,double? minDistance, double? maxDistance, int? minAge,int? maxAge, int? id)
        {
            if(String.IsNullOrEmpty(Lat)|| String.IsNullOrEmpty(Lng))
                  return (new ServiceResponseWithResult(false, "", null) { IsSuccess = false, Message = "Please provide valid Lat,Lng", Result = null });
            var result = new AccountService().NearByFriends(id,gender,minDistance,maxDistance,minAge,maxAge,Convert.ToDouble(Lat),Convert.ToDouble(Lng));
            result.Result = result.Result;
            return result;
        }
        [Route("profileimage/{id}")]
        [AuthorizeRequest]
        public ServiceResponseWithResult GetUserImage(int? id)
        {

            var result = new AccountService().UserImage(id);
            result.Result = result.Result;
            return result;
        }

        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

 

        // POST api/Account/Register
        /// <summary>
        /// Create Normal/Professional User
        /// </summary>
        /// <param name="model">TypeId ==> 1 for normal and 2 for professional</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [AuthorizeRequest]
        [Route("Create")]
        public  ServiceResponseWithResult Register(User model)
        {
            string msg = Validate(model);
            if (!String.IsNullOrEmpty(msg))
            {
                return (new ServiceResponseWithResult(false,msg,null) { IsSuccess = false, Message = msg,Result= null });
            }
            var result = new AccountService().CreateUser(model);
          
            return result;
        }
        [AllowAnonymous]
        [HttpPost]
        [AuthorizeRequest]
        [Route("AddDevice")]
        public ServiceResponseWithResult AddDevice(User model)
        {
           // string msg = Validate(model);
            if (String.IsNullOrEmpty(model.DeviceId))
            {
                return (new ServiceResponseWithResult(false, "DeviceId is required", null) { IsSuccess = false, Message = "DeviceId is required", Result = null });
            }
            if (String.IsNullOrEmpty(model.Platform))
            {
                return (new ServiceResponseWithResult(false, "Platform is required", null) { IsSuccess = false, Message = "Platform is required", Result = null });
            }
            var result = new AccountService().AddDevice(model);

            return result;
        }
        private string Validate(User model)
        {
           // File.AppendAllText("C://logs/temp/my.txt", "Success with" + "\r\n" + "Email=" + model.Email + ",User Name=" + model.UserName + ",FirstName=" + model.FirstName + ",Pass=" + model.PasswordHash);
            if (String.IsNullOrEmpty(model.Email))
            {
                return "Email is required";
            }
            if ( String.IsNullOrEmpty(model.FirstName))
            {
                return "First Name is required";
            }
            if (String.IsNullOrEmpty(model.LastName)
               )
            {
                return "Last Name is required";
            }
            //if (String.IsNullOrEmpty(model.DeviceId)
            //  )
            //{
            //    return "DeviceId is required";
            //}
            //if (String.IsNullOrEmpty(model.Platform)
            //  )
            //{
            //    return "Platform is required";
            //}
            //if (String.IsNullOrEmpty(model.Phone)
            // )
            //{
            //    return "Phone is required";
            //}
            if ( model.TypeId == null || model.TypeId <= 0 || model.TypeId > 2)
            {
                return "TypeId can be 1 0r 2";
            }
            return "";
        }
        [AllowAnonymous]
        [AuthorizeRequest]
        [Route("update")]
        public ServiceResponse Update(User model)
        {
            //if (String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.FirstName) || String.IsNullOrEmpty(model.PasswordHash)
            //    || model.TypeId == null || model.TypeId <= 0 || model.TypeId > 2)
            //{
            //    return (new ServiceResponse() { IsSuccess = false, Message = "Invalid input param" });
            //}

            var result = new AccountService().UpdateUser(model);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [AuthorizeRequest]
        [Route("SaveImage")]
        public ServiceResponseWithResult SaveImage(Dto.ResourceImages model)
        {
            if (String.IsNullOrEmpty(model.FileName) || String.IsNullOrEmpty(model.FileExtension) || String.IsNullOrEmpty(model.ImageType)
                || model.OwnerId <= 0 || model.UploadedBy <= 0)
            {
                return (new ServiceResponseWithResult(false, "Invalid input param",null) { IsSuccess = false, Message = "Invalid input param" });
            }
            Guid me = Guid.NewGuid();
            model.FileDirPath = HttpContext.Current.Server.MapPath("/avatar") + "\\" + me + "." + model.FileExtension;//.Split('.')[1];
            model.FileURL = ConfigurationManager.AppSettings["Url"] + "/avatar/" + me + "." + model.FileExtension;
            File.WriteAllBytes(model.FileDirPath, Convert.FromBase64String(model.Base64Data));
            var result = new AccountService().SaveImage(model);

            return result;
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
