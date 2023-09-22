namespace BitcoinPriceAggregator.Web.Exceptions
{
    /// <summary>
    /// Custom exception raised when scraping data from external data store fails.
    /// </summary>
    public class ScrapingFailedException : Exception
    {
        public ScrapingFailedException(string message) : base(message) { }
    }
}
