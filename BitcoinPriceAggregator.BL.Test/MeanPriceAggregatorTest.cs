using BitcoinPriceAggregator.Web.Exceptions;
using BitcoinPriceAggregator.Web.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BitcoinPriceAggregator.BL.Test
{
    public class MeanPriceAggregatorTest
    {
        private ILogger<MeanPriceAggregator> _loggerMock;
        private List<IPriceScraper> _scraperMocks;

        [SetUp]
        public void Init()
        {
            _loggerMock = Mock.Of<ILogger<MeanPriceAggregator>>();
            _scraperMocks = new List<IPriceScraper>();
            var bitfinexMock = new Mock<IPriceScraper>();
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 10))).ReturnsAsync(785.34f);
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 11))).ReturnsAsync(0);
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 12))).ReturnsAsync(785.34f);
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 13))).ReturnsAsync((float?)null);
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 14))).ReturnsAsync(0);
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 15))).ReturnsAsync(785.34f);
            bitfinexMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 16))).ThrowsAsync(new ScrapingFailedException(""));
            _scraperMocks.Add(bitfinexMock.Object);
            var bitstampMock = new Mock<IPriceScraper>();
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 10))).ReturnsAsync(652.19f);
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 11))).ReturnsAsync(652.19f);
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 12))).ReturnsAsync((float?)null);
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 13))).ReturnsAsync((float?)null);
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 14))).ReturnsAsync(0);
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 15))).ThrowsAsync(new ScrapingFailedException(""));
            bitstampMock.Setup(s => s.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 16))).ThrowsAsync(new ScrapingFailedException(""));
            _scraperMocks.Add(bitstampMock.Object);
        }

        [Test]
        public async Task GetPrice_Successful_ReturnsAggregatedPrice()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 10);
            Assert.IsNotNull(result);
            Assert.AreEqual(718.765f, result);
        }

        [Test]
        public async Task GetPrice_OneScraperReturnsZero_ReturnsAggregatedPrice()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 11);
            Assert.IsNotNull(result);
            Assert.AreEqual(326.095f, result);
        }

        [Test]
        public async Task GetPrice_OneScraperReturnsNull_IgnoresNullValue()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 12);
            Assert.IsNotNull(result);
            Assert.AreEqual(785.34f, result);
        }

        [Test]
        public async Task GetPrice_AllScrapersReturnNull_ReturnsNull()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 13);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetPrice_AllScrapersReturnZero_ReturnsZero()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 14);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task GetPrice_OneScraperThrowsException_ReturnsAggregatedPrice()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 15);
            Assert.IsNotNull(result);
            Assert.AreEqual(785.34f, result);
        }

        [Test]
        public async Task GetPrice_AllScrapersThrowExceptions_ReturnsNull()
        {
            var aggregator = new MeanPriceAggregator(_loggerMock, _scraperMocks);
            var result = await aggregator.GetAggregatedPriceAsync(new DateTime(2020, 4, 6), 16);
            Assert.IsNull(result);
        }
    }
}