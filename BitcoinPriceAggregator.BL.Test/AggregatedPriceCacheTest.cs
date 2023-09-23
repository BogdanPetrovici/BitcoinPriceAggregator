using BitcoinPriceAggregator.Data;
using Microsoft.Extensions.Logging;
using Moq;

namespace BitcoinPriceAggregator.BL.Test
{
    public class AggregatedPriceCacheTest
    {
        private ILogger<AggregatedPriceCache> _loggerMock;
        private Mock<IPriceAggregator> _priceAggregatorMock;
        private Mock<IPriceRepository> _priceRepositoryMock;

        [SetUp]
        public void Init()
        {
            _loggerMock = Mock.Of<ILogger<AggregatedPriceCache>>();
            _priceAggregatorMock = new Mock<IPriceAggregator>();
            _priceAggregatorMock.Setup(r => r.GetAggregatedPriceAsync(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 10))).ReturnsAsync(652.19f);
            _priceAggregatorMock.Setup(a => a.GetAggregatedPriceAsync(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 11))).ReturnsAsync(652.19f);
            _priceAggregatorMock.Setup(a => a.GetAggregatedPriceAsync(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 12))).ReturnsAsync((float?)null);

            _priceRepositoryMock = new Mock<IPriceRepository>();
            _priceRepositoryMock.Setup(r => r.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 10))).Returns(785.34f);
            _priceRepositoryMock.Setup(r => r.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 11))).Returns((float?)null);
            _priceRepositoryMock.Setup(r => r.GetPrice(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 12))).Returns((float?)null);
            _priceRepositoryMock.Setup(r => r.AddPriceAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<float>()));
        }

        [Test]
        public async Task GetPriceAsync_FoundPriceInCache_ReturnsCachedPrice()
        {
            var cache = new AggregatedPriceCache(_loggerMock, _priceAggregatorMock.Object, _priceRepositoryMock.Object);
            var result = await cache.GetPriceAsync(new DateTime(2020, 4, 6), 10);
            Assert.IsNotNull(result);
            Assert.AreEqual(785.34f, result);
        }

        [Test]
        public async Task GetPriceAsync_NoPriceInCache_ScrapesAggregatedPrice()
        {
            var cache = new AggregatedPriceCache(_loggerMock, _priceAggregatorMock.Object, _priceRepositoryMock.Object);
            var result = await cache.GetPriceAsync(new DateTime(2020, 4, 6), 11);
            Assert.IsNotNull(result);
            Assert.AreEqual(652.19f, result);
        }

        [Test]
        public async Task GetPriceAsync_NoPriceInCache_AddsScrapedPriceToCache()
        {
            var cache = new AggregatedPriceCache(_loggerMock, _priceAggregatorMock.Object, _priceRepositoryMock.Object);
            var result = await cache.GetPriceAsync(new DateTime(2020, 4, 6), 11);
            _priceRepositoryMock.Verify(mock => mock.AddPriceAsync(
                                                        It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), 
                                                        It.Is<int>(p => p == 11), 
                                                        It.Is<float>(p => p == 652.19f)), Times.Once());
        }

        [Test]
        public async Task GetPriceAsync_NoPriceInCacheOrExternalStore_ReturnsNull()
        {
            var cache = new AggregatedPriceCache(_loggerMock, _priceAggregatorMock.Object, _priceRepositoryMock.Object);
            var result = await cache.GetPriceAsync(new DateTime(2020, 4, 6), 12);
            Assert.IsNull(result);
        }
    }
}
