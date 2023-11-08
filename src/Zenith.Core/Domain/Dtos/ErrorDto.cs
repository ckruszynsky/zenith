using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Domain.Dtos
{
    public class ErrorDto
    {
        public ErrorDto(string description, object details = null)
        {
            Description = description;
            Details = details;
        }

        public string Description { get; }

        public object Details { get; set; }
    }
}
