namespace Zenith.Common.Date
{
    public interface IDateTime
    {
        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current date time year.
        /// </summary>
        int CurrentYear { get; }

        /// <summary>
        /// Gets the current date time month.
        /// </summary>
        int CurrentMonth { get; }

        /// <summary>
        /// Gets the current date time year.
        /// </summary>
        int CurrentDay { get; }
    }
}
