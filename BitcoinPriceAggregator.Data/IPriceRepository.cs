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
        Task AddPrice(DateTime date, int hour, float price);

        /// <summary>
        /// Gets the specified price point from the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point value, representing the aggregated price, if found in the local data store and null, otherwise</returns>
        Task<float?> GetPrice(DateTime date, int hour);
    }
}