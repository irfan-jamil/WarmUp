using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Common
{
    public class ServiceResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class ServiceResponseWithResult : ServiceResponse
    {
        public ServiceResponseWithResult(bool isSuccess, string message, object obj = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Result = obj;
        }

        public Object Result { get; set; }
    }
}
