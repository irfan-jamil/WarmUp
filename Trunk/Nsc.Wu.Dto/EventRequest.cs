using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class EventRequest
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<int> SentBy { get; set; }
        public Nullable<int> SentTo { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedUtcDate { get; set; }
        public Nullable<System.DateTime> UpdatedUtcDate { get; set; }
        public string Type { get; set; }

    }
}
