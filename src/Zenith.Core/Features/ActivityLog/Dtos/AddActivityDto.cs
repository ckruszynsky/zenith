using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.ActivityLog.Dtos
{
    public class AddActivityDto
    {
        public ActivityType ActivityType { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionId { get; set; }
    }
}
