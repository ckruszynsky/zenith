namespace Zenith.Common.Exceptions
{
    [Serializable]
    public class IdentityOperationException : Exception
    {
        public IdentityOperationException() : base()
        {

        }

        public IdentityOperationException(string message) : base(message)
        {

        }

        public IdentityOperationException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public IdentityOperationException(string[] messages) : base(string.Join("\n", messages))
        {

        }
    }
}
