using BitcoinPriceAggregator.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BitcoinPriceAggregator.Data
{
    public class InMemoryRepository : IPriceRepository
    {
        private PriceAggregatorContext _context;
        private ILogger _logger;

        public InMemoryRepository(ILogger<InMemoryRepository> logger, PriceAggregatorContext context)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Adds the specified price point to the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="DataLayerException">Raises custom exception when operation fails</exception>
        public async Task AddPriceAsync(DateTime date, int hour, float price)
        {
            try
            {
                _context.Prices.Add(new PricePointDbo(date, hour, price));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("An error occured while saving price date", ex);
                throw new DataLayerException("An error ocurred while persisting price point to local database");
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogError("The insert operation was canceled", ex);
                throw new DataLayerException("An error ocurred while persisting price point to local database");
            }
        }

        /// <summary>
        /// Gets the specified price point from the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point value, representing the aggregated price, if found in the local data store and null, otherwise</returns>
        public float? GetPrice(DateTime date, int hour)
        {
            var pricePoint = _context.Prices.Where(pricePoint => pricePoint.Hour == hour && pricePoint.Date == date).FirstOrDefault();
            return pricePoint != null ? pricePoint.Price : null;
        }

        /// <summary>
        /// Gets prices in the specified date range from the local data store
        /// </summary>
        /// <param name="startDate">The start date (UTC) for the time interval</param>
        /// <param name="startHour">The start hour for the time interval</param>
        /// <param name="endDate">The end date (UTC) for the time interval</param>
        /// <param name="endHour">The end hour for the time interval</param>
        /// <returns>A list of PricePoint DTOs containing the prices found in the database for the specified time range</returns>
        public IEnumerable<PricePointDbo> GetPrices(DateTime startDate, int startHour, DateTime endDate, int endHour)
        {
            var pricePoints = _context.Prices.Where(pricePoint => pricePoint.Date >= startDate && pricePoint.Hour >= startHour &&
                                                                  pricePoint.Date <= endDate && pricePoint.Hour <= endHour);
            return pricePoints;
        }
    }
}