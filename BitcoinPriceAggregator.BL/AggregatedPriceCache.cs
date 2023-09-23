using BitcoinPriceAggregator.Data;

namespace BitcoinPriceAggregator.BL
{
    public class AggregatedPriceCache : IPriceCache
    {
        IPriceAggregator _priceAggregator;
        IPriceRepository _priceRepository;

        public AggregatedPriceCache(IPriceAggregator priceAggregator, IPriceRepository priceRepository)
        {
            _priceAggregator = priceAggregator;
            _priceRepository = priceRepository;
        }

        /// <summary>
        /// Tries getting the requested aggregated price from the local data store. 
        /// If not found, tries retrieving it from the external data sources and updating the local data store.
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The time for the price point</param>
        /// <returns>A floating point number representing the aggregated price, or null, if the price was not available the local data store or any of the external data stores</returns>
        public async Task<float?> GetPrice(DateTime date, int hour)
        {
            var persistedPrice = await _priceRepository.GetPrice(date, hour);
            if (persistedPrice.HasValue)
            {
                persistedPrice = await _priceAggregator.GetAggregatedPrice(date, hour);
                if (persistedPrice.HasValue)
                {
                    await _priceRepository.AddPrice(date, hour, persistedPrice.Value);
                    return persistedPrice.Value;
                }

            }

            return null;
        }
    }
}
