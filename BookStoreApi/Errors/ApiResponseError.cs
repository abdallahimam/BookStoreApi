using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Errors
{
    public class ApiResponseError
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponseError(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultErrorMessage(statusCode);
        }

        private string GetDefaultErrorMessage(int statusCode) {

            return statusCode switch {
                400 => "Bad request.",
                401 => "An Authorized request.",
                404 => "Not found page/request",
                500 => "Server error has beed occured.",
                _ => "An error occured during request."
            };
        }
    }
}