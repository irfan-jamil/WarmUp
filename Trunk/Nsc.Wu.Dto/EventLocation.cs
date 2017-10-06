using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class EventLocation
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string MarkerColor { get; set; }
        public string Address { get; set; }
    }
}
