namespace Zenith.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmptyString(this string value)
        {
            return value != null && value.Length == 0;
        }
    }
}
