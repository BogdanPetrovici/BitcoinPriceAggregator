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
        Task<float?> GetPriceAsync(DateTime date, int hour);

        /// <summary>
        /// Gets prices in the specified date range from the local data store
        /// </summary>
        /// <param name="startDate">The start date (UTC) for the time interval</param>
        /// <param name="startHour">The start hour for the time interval</param>
        /// <param name="endDate">The end date (UTC) for the time interval</param>
        /// <param name="endHour">The end hour for the time interval</param>
        /// <returns>A list of PricePoint DTOs containing the prices found in the database for the specified time range</returns>
        IEnumerable<PricePoint> GetPrices(DateTime startDate, int startHour, DateTime endDate, int endHour);
    }
}
