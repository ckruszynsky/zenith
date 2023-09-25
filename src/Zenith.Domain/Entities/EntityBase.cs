namespace Zenith.SharedKernel
{
    // <summary>
    /// A base class for DDD Entities. Includes support for domain events dispatched post-persistence.
    /// </summary>
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
