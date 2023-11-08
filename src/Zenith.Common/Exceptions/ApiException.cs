using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Common.Exceptions
{
        using System;
        using System.Collections.Generic;
        using System.Net;
        
        public class ApiException : Exception
        {
            public ApiException(string message, HttpStatusCode statusCode)
                : base(message)
            {
                StatusCode = statusCode;
                ApiErrors = new List<ApiError>() { new ApiError(message) };                
            }

            public ApiException(string message, HttpStatusCode statusCode, ICollection<ApiError> apiErrors)
                : base(message)
            {
                StatusCode = statusCode;
                ApiErrors = apiErrors;                
            }

            public object Errors { get; set; }

            public HttpStatusCode StatusCode { get; }

            public ICollection<ApiError> ApiErrors { get; }            
        }
    }
