using BitcoinPriceAggregator.Web.Interfaces;
using Microsoft.Extensions.Logging;

namespace BitcoinPriceAggregator.BL
{
    /// <summary>
    /// Aggregates closing prices from supplied external data sources by averaging their vaules
    /// </summary>
    public class MeanPriceAggregator : IPriceAggregator
    {
        private IEnumerable<IPriceScraper> _priceRepositories;
        private ILogger _logger;

        public MeanPriceAggregator(ILogger<MeanPriceAggregator> logger, IEnumerable<IPriceScraper> priceRepositories)
        {
            _priceRepositories = priceRepositories;
            _logger = logger;
        }

        /// <summary>
        /// Gets the aggregated price from multiple external data sources
        /// </summary>
        /// <param name="startHourUtc">The date and time (in hours, UTC) for the price point</param>
        /// <returns>A floating point number representing the aggregated price, or null, if the price was not available in any of the external data stores</returns>
        public async Task<float?> GetAggregatedPrice(DateTime startHourUtc)
        {
            float sum = 0;
            int priceCount = 0;
            foreach (var repository in _priceRepositories)
            {
                try
                {
                    var price = await repository.GetPrice(startHourUtc);
                    if (price != null)
                    {
                        sum = sum + price.Value;
                        priceCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(string.Format("Could not retrieve price from external data source: {0}", repository.GetType().FullName), ex);
                }
            }

            return priceCount > 0 ? sum / priceCount : null;
        }
    }
}