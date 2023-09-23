namespace BitcoinPriceAggregator.Data.Exceptions
{
    /// <summary>
    /// Custom exception raised when querying local data store fails.
    /// </summary>
    public class DataLayerException : Exception
    {
        public DataLayerException(string message) : base(message) { }
    }
}
