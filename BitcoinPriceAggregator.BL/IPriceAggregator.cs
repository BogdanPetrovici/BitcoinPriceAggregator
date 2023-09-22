
namespace BitcoinPriceAggregator.BL
{
    public interface IPriceAggregator
    {
        /// <summary>
        /// Gets the aggregated price from multiple external data sources
        /// </summary>
        /// <param name="startHourUtc">The date and time (in hours, UTC) for the price point</param>
        /// <returns>A floating point number representing the aggregated price, or null, if the price was not available in any of the external data stores</returns>
        Task<float?> GetAggregatedPrice(DateTime startHourUtc);
    }
}
