using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class User
    {
        public int Id { get; set; }
   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Rating { get; set; }
        public string About { get; set; }
        public string Gender { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public string DeviceId { get; set; }
        public string Platform { get; set; }
        public List<string> UserImages { get; set; }
        public string ProfilePic { get; set; }
        public Nullable<System.DateTime> Dob { get; set; }
        public string DobUnix { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<byte> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedUtcDate { get; set; }
        public Nullable<System.DateTime> UpdatedUtcDate { get; set; }
    }
}
