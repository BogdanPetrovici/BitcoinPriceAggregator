namespace BitcoinPriceAggregator.Data
{
    public class InMemoryRepository : IPriceRepository
    {
        private PriceAggregatorContext _context;

        public InMemoryRepository(PriceAggregatorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the specified price point to the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task AddPrice(DateTime date, int hour, float price)
        {
            _context.Prices.Add(new PricePoint(date, hour, price));
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the specified price point from the local data store
        /// </summary>
        /// <param name="date">The date (UTC) for the price point</param>
        /// <param name="hour">The hour for the price point</param>
        /// <returns>A floating point value, representing the aggregated price, if found in the local data store and null, otherwise</returns>
        public async Task<float?> GetPrice(DateTime date, int hour)
        {
            var pricePoint = _context.Prices.Where(pricePoint => pricePoint.Hour == hour && pricePoint.Date == date).FirstOrDefault();
            return pricePoint != null ? pricePoint.Price : null;
        }
    }
}
