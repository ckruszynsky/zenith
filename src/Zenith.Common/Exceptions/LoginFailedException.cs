namespace Zenith.Common.Exceptions
{
    [Serializable]
    public class LoginFailedException : Exception
    {
        public LoginFailedException() : base(message: "Login Failed. Please check the username or password and try again.")
        {

        }
    }
}
