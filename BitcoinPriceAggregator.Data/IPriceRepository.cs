namespace BitcoinPriceAggregator.Data
{
    public interface IPriceRepository
    {
        /// <summary>
        /// Adds the specified price point to the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="DataLayerException">Raises custom exception when operation fails</exception>
        Task AddPriceAsync(DateTime date, int hour, float price);

        /// <summary>
        /// Gets the specified price point from the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point value, representing the aggregated price, if found in the local data store and null, otherwise</returns>
        float? GetPrice(DateTime date, int hour);

        /// <summary>
        /// Gets prices in the specified date range from the local data store
        /// </summary>
        /// <param name="startDate">The start date (UTC) for the time interval</param>
        /// <param name="startHour">The start hour for the time interval</param>
        /// <param name="endDate">The end date (UTC) for the time interval</param>
        /// <param name="endHour">The end hour for the time interval</param>
        /// <returns>A list of PricePoint DBOs containing the prices found in the database for the specified time range</returns>
        IEnumerable<PricePointDbo> GetPrices(DateTime startDate, int startHour, DateTime endDate, int endHour);
    }
}