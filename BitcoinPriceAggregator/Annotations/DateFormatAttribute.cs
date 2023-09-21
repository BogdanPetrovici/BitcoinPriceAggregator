using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BitcoinPriceAggregator.Api.Annotations
{
    public class DateFormatAttribute : ValidationAttribute
    {
        private readonly string _format;

        public DateFormatAttribute(string format)
            : base($"Date should be in {format} format.")
        {
            _format = format;
        }

        public override bool IsValid(object value)
        {
            if (value is not string dateStr)
            {
                return false;
            }

            return DateTime.TryParseExact(dateStr, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}