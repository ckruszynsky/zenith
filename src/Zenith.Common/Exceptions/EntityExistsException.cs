namespace Zenith.Common.Exceptions
{
    [Serializable]
    public class EntityExistsException : Exception
    {
        public EntityExistsException() : base() { }
        public EntityExistsException(string message) : base(message) { }
        public EntityExistsException(string message, Exception innerException) : base(message, innerException) { }
        public EntityExistsException(string name, object key)
            : base($"Entity with \"{name}\" with ({key}) already exists.")
        {
        }
    }
}
