using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class NotificationDetailDto
    {
        public string body { get; set; }
        public string type { get; set; }
        public bool isRead { get; set; }
        public long id { get; set; }
    }
}
