using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class UserRatingDto
    {
 
        public Nullable<int> EventId { get; set; }
        public Nullable<int> userid { get; set; }
        public Nullable<int> Rating { get; set; }
        public string Comment { get; set; }
        public Nullable<int> FromUserId { get; set; }

     
    }
}
