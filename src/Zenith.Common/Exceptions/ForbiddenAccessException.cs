namespace Zenith.Common.Exceptions
{
    [Serializable]
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message):base(message) { }
        
        public ForbiddenAccessException() : base() { }
    }
}
