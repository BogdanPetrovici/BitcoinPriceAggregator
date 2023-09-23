using BitcoinPriceAggregator.Data;
using BitcoinPriceAggregator.Data.Exceptions;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BitcoinPriceAggregator.BL
{
    public class AggregatedPriceCache : IPriceCache
    {
        IPriceAggregator _priceAggregator;
        IPriceRepository _priceRepository;
        ILogger _logger;

        public AggregatedPriceCache(ILogger<AggregatedPriceCache> logger, IPriceAggregator priceAggregator, IPriceRepository priceRepository)
        {
            _priceAggregator = priceAggregator;
            _priceRepository = priceRepository;
            _logger = logger;
        }

        /// <summary>
        /// Tries getting the requested aggregated price from the local data store. 
        /// If not found, tries retrieving it from the external data sources and updating the local data store.
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The time for the price point</param>
        /// <returns>A floating point number representing the aggregated price, or null, if the price was not available the local data store or any of the external data stores</returns>
        public async Task<float?> GetPriceAsync(DateTime date, int hour)
        {
            var persistedPrice = _priceRepository.GetPrice(date, hour);
            if (persistedPrice.HasValue)
            {
                return persistedPrice.Value;
            }


            persistedPrice = await _priceAggregator.GetAggregatedPriceAsync(date, hour);
            if (persistedPrice.HasValue)
            {
                try
                {
                    await _priceRepository.AddPriceAsync(date, hour, persistedPrice.Value);
                }
                catch (DataLayerException ex)
                {
                    _logger.LogError("Could not persist price data to database", ex);
                }

                return persistedPrice.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets prices in the specified date range from the local data store
        /// </summary>
        /// <param name="startDate">The start date (UTC) for the time interval</param>
        /// <param name="startHour">The start hour for the time interval</param>
        /// <param name="endDate">The end date (UTC) for the time interval</param>
        /// <param name="endHour">The end hour for the time interval</param>
        /// <returns>A list of PricePoint DTOs containing the prices found in the database for the specified time range</returns>
        public IEnumerable<PricePoint> GetPrices(DateTime startDate, int startHour, DateTime endDate, int endHour)
        {
            var persistedPricePoints = _priceRepository.GetPrices(startDate, startHour, endDate, endHour);
            return persistedPricePoints?.Select(pricePoint => new PricePoint(pricePoint.Date, pricePoint.Hour, pricePoint.Price));
        }
    }
}
