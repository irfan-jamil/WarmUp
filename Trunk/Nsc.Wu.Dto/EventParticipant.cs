using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class EventParticipant
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<int> ParticipantId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Type { get; set; }
    }
}
