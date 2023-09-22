using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BitcoinPriceAggregator.Api.Annotations
{
    /// <summary>
    /// Used for validating string parameters representing date and time in controllers
    /// </summary>
    public class DateFormatAttribute : ValidationAttribute
    {
        private readonly string _format;

        public DateFormatAttribute(string format)
            : base($"Date should be in {format} format.")
        {
            _format = format;
        }

        /// <summary>
        /// Checks whether the supplied value is a string and, if so, whether it represents a timestamp with the required formatting
        /// </summary>
        /// <param name="value">The value to be validated</param>
        /// <returns>True if string representing properly formatted timestamp, false otherwise</returns>
        public override bool IsValid(object? value)
        {
            if (value is not string dateStr)
            {
                return false;
            }

            return DateTime.TryParseExact(dateStr, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}