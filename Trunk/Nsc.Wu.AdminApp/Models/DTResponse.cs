using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nsc.Wu.AdminApp.Models
{
    public class DTResponse
    {
        public string recordsTotal { get; set; }
        public string recordsFiltered { get; set; }
        public string length { get; set; }
        public string draw { get; set; }
        public List<EventDto> data { get; set; }
    }
}