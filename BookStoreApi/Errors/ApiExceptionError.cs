using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Errors
{
    public class ApiExceptionError : ApiResponseError
    {
        public string Details { get; set; }

        public ApiExceptionError(int statusCode, string message = null, string details = null) : base(statusCode, message)
        {
            Details = details;
        }
    }
}