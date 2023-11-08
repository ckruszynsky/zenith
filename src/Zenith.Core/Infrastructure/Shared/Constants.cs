using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Zenith.Core.Infrastructure.Shared
{
    public static class Constants
    {
        // Application Constants
        public const string ApiVersion = "0.1.0-alpha";

        // JSON Serialization Settings
        public static readonly JsonSerializerSettings ConduitJsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
    }
}
