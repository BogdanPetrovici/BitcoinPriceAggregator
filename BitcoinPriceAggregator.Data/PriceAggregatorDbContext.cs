using Microsoft.EntityFrameworkCore;

namespace BitcoinPriceAggregator.Data
{
    public class PriceAggregatorContext : DbContext
    {
        public DbSet<PricePoint> Prices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Prices");
        }
    }
}
