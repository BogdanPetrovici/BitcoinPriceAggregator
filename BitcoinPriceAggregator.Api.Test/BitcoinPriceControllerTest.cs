using BitcoinPriceAggregator.Api.Controllers;
using BitcoinPriceAggregator.BL;
using BitcoinPriceAggregator.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BitcoinPriceAggregator.Api.Test
{
    public class BitcoinPriceControllerTest
    {
        private ILogger<BitcoinPriceController> _loggerMock;
        private Mock<IPriceCache> _cacheMock;
        private List<PricePoint> _pricePoints;

        [SetUp]
        public void Setup()
        {
            _loggerMock = Mock.Of<ILogger<BitcoinPriceController>>();
            _pricePoints = new List<PricePoint>() {
                new PricePoint(new DateTime(2020, 4, 6), 21, 785.34f),
                new PricePoint(new DateTime(2020, 4, 6), 10, 652.19f),
                new PricePoint(new DateTime(2020, 4, 6), 15, 434.82f)
            };

            _cacheMock = new Mock<IPriceCache>();
            _cacheMock.Setup(c => c.GetPriceAsync(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 10))).ReturnsAsync(785.34f);
            _cacheMock.Setup(c => c.GetPriceAsync(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 11))).ThrowsAsync(new Exception());
            _cacheMock.Setup(c => c.GetPriceAsync(It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 12))).ReturnsAsync((float?)null);

            _cacheMock.Setup(c => c.GetPrices(
                It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 10),
                It.Is<DateTime>(p => p == new DateTime(2020, 4, 6)), It.Is<int>(p => p == 23)
            )).Returns(_pricePoints);
            _cacheMock.Setup(c => c.GetPrices(
                It.Is<DateTime>(p => p == new DateTime(2020, 4, 7)), It.Is<int>(p => p == 10),
                It.Is<DateTime>(p => p == new DateTime(2020, 4, 7)), It.Is<int>(p => p == 23)
            )).Throws(new Exception());
        }

        [Test]
        public async Task GetUsdPrice_Successful_ReturnsOk()
        {
            var controller = new BitcoinPriceController(_loggerMock, _cacheMock.Object);
            var response = await controller.GetUsdPrice("2020040610Z");
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            Assert.AreEqual(785.34f, ((OkObjectResult)response.Result).Value);
        }

        [Test]
        public async Task GetUsdPrice_CacheReturnsNull_ReturnsOkAndNull()
        {
            var controller = new BitcoinPriceController(_loggerMock, _cacheMock.Object);
            var response = await controller.GetUsdPrice("2020040612Z");
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            Assert.IsNull(((OkObjectResult)response.Result).Value);
        }

        [Test]
        public async Task GetUsdPrice_CacheThrowsException_ReturnsStatusCode500()
        {
            var controller = new BitcoinPriceController(_loggerMock, _cacheMock.Object);
            var response = await controller.GetUsdPrice("2020040611Z");
            Assert.IsInstanceOf<ObjectResult>(response.Result);
            Assert.AreEqual(((ObjectResult)response.Result).StatusCode, 500);
        }

        [Test]
        public void GetPrices_Successful_ReturnsOk()
        {
            var controller = new BitcoinPriceController(_loggerMock, _cacheMock.Object);
            var response = controller.GetUsdPrices("2020040610Z", "2020040623Z");
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
        }

        [Test]
        public void GetPrices_CacheThrowsException_ReturnsStatusCode500()
        {
            var controller = new BitcoinPriceController(_loggerMock, _cacheMock.Object);
            var response = controller.GetUsdPrices("2020040710Z", "2020040723Z");
            Assert.IsInstanceOf<ObjectResult>(response.Result);
            Assert.AreEqual(((ObjectResult)response.Result).StatusCode, 500);
        }
    }
}