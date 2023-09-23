using Microsoft.EntityFrameworkCore;

namespace BitcoinPriceAggregator.Data
{
    public class PriceAggregatorContext : DbContext
    {
        public virtual DbSet<PricePointDbo> Prices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Prices");
        }
    }
}
