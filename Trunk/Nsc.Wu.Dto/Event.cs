using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Address { get; set; }
        public bool IsFree { get; set; }
        public string PaymentFee { get; set; }
        public string Code { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedUtcDate { get; set; }
        public Nullable<System.DateTime> UpdatedUtcDate { get; set; }
        public long EventStartDateUnix { get; set; }
        public long EventEndDateUnix { get; set; }
        //public User UserAccount { get; set; }
        public int MaxParticipants { get; set; }
        public  List<EventLocation> EventLocations { get; set; }
        public UserProfleDto Creater { get; set; }
        public List<EventLable> EventLables { get; set; }
        public List<UserProfleDto> EventParticipants { get; set; }
       
        public List<UserProfleDto> EventRequests { get; set; }

        public List<String> EventImages { get; set; }
       
    }
}
