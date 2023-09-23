namespace BitcoinPriceAggregator.BL
{
    public class PricePoint
    {
        public PricePoint() { }
        public PricePoint(DateTime date, int hour, float price)
        {
            Date = date;
            Hour = hour;
            Price = price;
        }

        public DateTime Date { get; set; }
        public int Hour { get; set; }
        public float Price { get; set; }
    }
}
