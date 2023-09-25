using Zenith.Common.Date;

namespace Zenith.Core.Infrastructure.Services
{
    public class MachineDateTime : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;

        public int CurrentYear => DateTime.Now.Year;

        public int CurrentMonth => DateTime.Now.Month;

        public int CurrentDay => DateTime.Now.Day;
    }
}
