﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public  class EventRequestDTO
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
    }
}
