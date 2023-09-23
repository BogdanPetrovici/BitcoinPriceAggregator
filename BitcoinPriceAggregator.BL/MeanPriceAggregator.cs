using BitcoinPriceAggregator.Web.Interfaces;
using Microsoft.Extensions.Logging;

namespace BitcoinPriceAggregator.BL
{
    /// <summary>
    /// Aggregates closing prices from supplied external data sources by averaging their vaules
    /// </summary>
    public class MeanPriceAggregator : IPriceAggregator
    {
        private IEnumerable<IPriceScraper> _priceScrapers;
        private ILogger _logger;

        public MeanPriceAggregator(ILogger<MeanPriceAggregator> logger, IEnumerable<IPriceScraper> priceScrapers)
        {
            _priceScrapers = priceScrapers;
            _logger = logger;
        }

        /// <summary>
        /// Gets the aggregated price from multiple external data sources
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point number representing the aggregated price, or null, if the price was not available in any of the external data stores</returns>
        public async Task<float?> GetAggregatedPrice(DateTime date, int hour)
        {
            float sum = 0;
            int priceCount = 0;
            foreach (var scraper in _priceScrapers)
            {
                try
                {
                    var price = await scraper.GetPrice(date, hour);
                    if (price != null)
                    {
                        sum = sum + price.Value;
                        priceCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(string.Format("Could not retrieve price from external data source: {0}", scraper.GetType().FullName), ex);
                }
            }

            return priceCount > 0 ? sum / priceCount : null;
        }
    }
}