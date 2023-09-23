using BitcoinPriceAggregator.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;

namespace BitcoinPriceAggregator.Data.Test
{
    public class TestInMemoryRepository
    {
        private Mock<DbSet<PricePointDbo>> _dbSetPricesMock;
        private Mock<PriceAggregatorContext> _dbContextMock;
        private Mock<PriceAggregatorContext> _dbContextMockUpdateFail;
        private Mock<PriceAggregatorContext> _dbContextCancelled;
        private ILogger<InMemoryRepository> _loggerMock;
        private List<PricePointDbo> _pricePoints;

        [SetUp]
        public void Init()
        {
            _loggerMock = Mock.Of<ILogger<InMemoryRepository>>();
            _pricePoints = new List<PricePointDbo>() {
                new PricePointDbo(new DateTime(2020, 4, 6), 21, 785.34f),
                new PricePointDbo(new DateTime(2020, 4, 6), 10, 652.19f),
                new PricePointDbo(new DateTime(2020, 4, 6), 15, 434.82f)
            };
            _dbSetPricesMock = new Mock<DbSet<PricePointDbo>>();
            _dbSetPricesMock.As<IQueryable<PricePointDbo>>().Setup(x => x.Provider).Returns(_pricePoints.AsQueryable().Provider);
            _dbSetPricesMock.As<IQueryable<PricePointDbo>>().Setup(x => x.Expression).Returns(_pricePoints.AsQueryable().Expression);
            _dbSetPricesMock.As<IQueryable<PricePointDbo>>().Setup(x => x.ElementType).Returns(_pricePoints.AsQueryable().ElementType);
            _dbSetPricesMock.As<IQueryable<PricePointDbo>>().Setup(x => x.GetEnumerator()).Returns(_pricePoints.AsQueryable().GetEnumerator());
            _dbSetPricesMock.Setup(x => x.Add(It.IsAny<PricePointDbo>())).Callback<PricePointDbo>((pricePoint) => _pricePoints.Add(pricePoint));

            _dbContextMock = new Mock<PriceAggregatorContext>();
            _dbContextMock.Setup(item => item.Prices).Returns(_dbSetPricesMock.Object);

            _dbContextMockUpdateFail = new Mock<PriceAggregatorContext>();
            _dbContextMockUpdateFail.Setup(item => item.Prices).Returns(_dbSetPricesMock.Object);
            _dbContextMockUpdateFail.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
                                    .ThrowsAsync(new DataLayerException(""));

            _dbContextCancelled = new Mock<PriceAggregatorContext>();
            _dbContextCancelled.Setup(item => item.Prices).Returns(_dbSetPricesMock.Object);
            _dbContextCancelled.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
                                    .ThrowsAsync(new OperationCanceledException(""));
        }

        [Test]
        public void GetPrice_PriceFound_ReturnsPriceValue()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.GetPrice(new DateTime(2020, 4, 6), 10);
            Assert.AreEqual(652.19f, result.Value);
        }

        [Test]
        public void GetPrice_PriceNotFound_ReturnsNull()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.GetPrice(new DateTime(2020, 4, 7), 10);
            Assert.IsNull(result);
        }

        [Test]
        public void GetPrices_PricesInRange_ReturnsApplicablePrices()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.GetPrices(new DateTime(2020, 4, 6), 0, new DateTime(2020, 4, 6), 16).ToList();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(652.19f, result[0].Price);
            Assert.AreEqual(434.82f, result[1].Price);
        }

        [Test]
        public void GetPrices_PricesInRange_ReturnsEmptyList()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextMock.Object);
            var result = repository.GetPrices(new DateTime(2020, 4, 7), 0, new DateTime(2020, 4, 7), 23).ToList();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task AddPrice_UpdateError_ThrowsException()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextMockUpdateFail.Object);
            Assert.ThrowsAsync<DataLayerException>(async () => await repository.AddPriceAsync(new DateTime(2020, 4, 8), 10, 123.45f));
        }

        [Test]
        public async Task AddPrice_Cancelled_ThrowsException()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextCancelled.Object);
            Assert.ThrowsAsync<DataLayerException>(async () => await repository.AddPriceAsync(new DateTime(2020, 4, 8), 10, 123.45f));
        }

        [Test]
        public async Task AddPrice_Successful_UpdatesPriceList()
        {
            var repository = new InMemoryRepository(_loggerMock, _dbContextMock.Object);
            await repository.AddPriceAsync(new DateTime(2020, 4, 8), 10, 123.45f);
            Assert.AreEqual(4, _pricePoints.Count);
            var newPrice = _pricePoints.Where(price => price.Date == new DateTime(2020, 4, 8) && price.Hour == 10).FirstOrDefault();
            Assert.IsNotNull(newPrice);
            Assert.AreEqual(123.45f, newPrice.Price);
        }
    }
}