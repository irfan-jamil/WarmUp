using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nsc.Wu.AdminApp.Models
{
    public class DTSearchParam
    {
      
        public string Search { get; set; }

        public string OrderBy { get; set; }
        public string Dir { get; set; }
        public int Start { get; set; } = 0;
        public int End { get; set; } = 0;
        public int Draw { get; set; } = 0;
        public int Length { get; set; } = 0;
    }
}