using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Common.Exceptions
{
    public class ApiError
    {
        public ApiError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ApiError(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }

        public string ErrorMessage { get; }

        public string PropertyName { get; }
    }
}
