using MediatR;

namespace Zenith.SharedKernel
{
    /// <summary>
    /// A base type for domain events
    /// </summary>
    public abstract class DomainEventBase : INotification
    {
        /// <summary>
        /// Date event occurred which is set on creation
        /// </summary>
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }

}
