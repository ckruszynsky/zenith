using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Domain.Dtos;

namespace Zenith.Core.Domain.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel(ErrorDto errors)
        {
            Errors = errors;
        }

        public ErrorDto Errors { get; }
    }
}
