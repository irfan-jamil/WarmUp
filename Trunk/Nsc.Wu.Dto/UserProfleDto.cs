using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class UserProfleDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<byte> IsActive { get; set; }
        public string Rating { get; set; }
        public string About { get; set; }
        public string Gender { get; set; }
        public String DobUnix { get; set; }
        public String Type { get; set; }

        public PaymentDto Payment { get; set; }
        public Nullable<System.DateTime> Dob { get; set; }
        public Nullable<System.DateTime> CreatedUtcDate { get; set; }
        public Nullable<System.DateTime> UpdatedUtcDate { get; set; }
    }
}
