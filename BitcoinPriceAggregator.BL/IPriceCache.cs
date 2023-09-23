namespace BitcoinPriceAggregator.BL
{
    public interface IPriceCache
    {
        /// <summary>
        /// Tries getting the requested aggregated price from the local data store. 
        /// If not found, tries retrieving it from the external data sources and updating the local data store.
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The time for the price point</param>
        /// <returns>A floating point number representing the aggregated price, or null, if the price was not available the local data store or any of the external data stores</returns>
        Task<float?> GetPrice(DateTime date, int hour);
    }
}
