using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Infrastructure.Shared
{
    public static class ErrorMessages
    {
        public const string InternalServerError = "An unexpected error has occurred, please try the request again.";
        public const string ValidationError = "One or more validation errors has occurred during the request, please fix the errors and try again.";
    }
}
