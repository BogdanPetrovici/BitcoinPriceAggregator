
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitcoinPriceAggregator.Data
{
    [PrimaryKey("Date", "Hour")]
    public class PricePointDbo
    {
        public PricePointDbo() { }
        public PricePointDbo(DateTime date, int hour, float price)
        {
            Date = date;
            Hour = hour;
            Price = price;
        }

        [Key, Column(Order = 0)]
        public DateTime Date { get; set; }
        [Key, Column(Order = 1)]
        public int Hour { get; set; }
        [Column(Order = 2)]
        public float Price { get; set; }
    }
}
