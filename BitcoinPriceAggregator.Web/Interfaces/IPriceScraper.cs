namespace BitcoinPriceAggregator.Web.Interfaces
{
    public interface IPriceScraper
    {
        /// <summary>
        /// Scrapes the price for the given hour (on the given date) from an external data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point number representing the price, or null, if the price was not available</returns>
        /// <exception cref="BitcoinPriceAggregator.Web.Exceptions.ScrapingFailedException">Thrown when retrieval of data point failed</exception>
        Task<float?> GetPrice(DateTime date, int hour);
    }
}
