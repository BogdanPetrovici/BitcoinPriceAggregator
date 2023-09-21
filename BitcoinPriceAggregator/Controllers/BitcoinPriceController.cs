using BitcoinPriceAggregator.Api.Annotations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BitcoinPriceAggregator.Api.Controllers
{
    [Route("prices")]
    [ApiController]
    public class BitcoinPriceController : ControllerBase
    {
        [HttpGet("btcusd/{startHourUtc}", Name = "GetSinglePrice")]
        public ActionResult<float> GetUsdPrice([DateFormat(format: "yyyyMMddHHZ")] string startHourUtc)
        {
            DateTime startHour = DateTime.ParseExact(startHourUtc, "yyyyMMddHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            return Ok(float.MinValue);
        }

        [HttpGet("btcusd", Name = "GetCachedPrices")]
        public ActionResult<List<float>> GetUsdPrices([Required][DateFormat(format: "yyyyMMddHHZ")] string startHourUtc, [Required][DateFormat(format: "yyyyMMddHHZ")] string endHourUtc)
        {
            DateTime startHour = DateTime.ParseExact(startHourUtc, "yyyyMMddHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            DateTime endHour = DateTime.ParseExact(endHourUtc, "yyyyMMddHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            return Ok(new List<float> { float.MinValue, float.MinValue, float.MinValue });
        }
    }
}
