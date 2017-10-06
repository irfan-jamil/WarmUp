using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public  class EventLable
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public string Label { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedUTCDateTime { get; set; }
        public Nullable<System.DateTime> UpdatedUTCDateTime { get; set; }

    
    }
}
