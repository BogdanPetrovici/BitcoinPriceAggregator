using BitcoinPriceAggregator.Api.Annotations;
using BitcoinPriceAggregator.BL;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BitcoinPriceAggregator.Api.Controllers
{
    [Route("prices")]
    [ApiController]
    public class BitcoinPriceController : ControllerBase
    {
        private readonly ILogger<BitcoinPriceController> _logger;
        private IPriceCache _priceCache;

        public BitcoinPriceController(ILogger<BitcoinPriceController> logger, IPriceCache priceCache)
        {
            _logger = logger;
            _priceCache = priceCache;
        }

        /// <summary>
        /// Gets the aggregated Bitcoin price in USD for a specific hour at a specific date. 
        /// If the value is not found in the local data storage, it is retrieved from the external providers, aggregated, then stored in the local data storage.
        /// </summary>
        /// <param name="startHourUtc">String representing the date and hour, in UTC</param>
        /// <returns>Successful response message with data point as serialized floating point number if no errors, internal server error response message otherwise</returns>
        /// <response code="200">Returns the requested price value either from cache or from external data sources</response>
        /// <response code="400">If the supplied date is not in the expected format (or not a date)</response>
        [HttpGet("btcusd/{startHourUtc}", Name = "GetSinglePrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<ActionResult<float>> GetUsdPrice(
            [DateFormat(format: "yyyyMMddHHZ")]
            string startHourUtc)
        {
            try
            {
                DateTime startHour = DateTime.ParseExact(startHourUtc, "yyyyMMddHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                float? persistedPrice = await _priceCache.GetPriceAsync(startHour.Date, startHour.Hour);
                return Ok(persistedPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not retrieve aggregated price", ex);
                return StatusCode(500, "An error occured while retrieving the aggregated price");
            }

        }

        /// <summary>
        /// Gets the stored aggregated bitcoin prices in USD for a time interval
        /// </summary>
        /// <param name="startHourUtc">Start of the time interval as UTC date and hour</param>
        /// <param name="endHourUtc">End of the interval as UTC date and hour</param>
        /// <returns>A list of floating point values representing the aggregated prices for each hour in the interval. If an hour doesn't have a recorded price, it will have a null value</returns>
        /// <response code="200">Returns the requested price values from cache</response>
        /// <response code="400">If the supplied date is not in the expected format (or not a date)</response>
        [HttpGet("btcusd", Name = "GetCachedPrices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public ActionResult<IEnumerable<PricePoint>> GetUsdPrices(
            [Required]
            [DateFormat(format: "yyyyMMddHHZ")]
            string startHourUtc,
            [Required]
            [DateFormat(format: "yyyyMMddHHZ")]
            string endHourUtc)
        {
            try
            {
                DateTime startHour = DateTime.ParseExact(startHourUtc, "yyyyMMddHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                DateTime endHour = DateTime.ParseExact(endHourUtc, "yyyyMMddHHZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                var pricePoints = _priceCache.GetPrices(startHour.Date, startHour.Hour, endHour.Date, endHour.Hour);
                return Ok(pricePoints);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not retrieve aggregated prices", ex);
                return StatusCode(500, "An error occured while retrieving the aggregated prices");
            }

        }
    }
}
