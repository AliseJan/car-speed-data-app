using CarStatsApp.Services;
using CarStatsApp.Data;
using Moq;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using CarStatsApp.Core.Models;
using CarStatsApp.Core.Services;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace CarStatsApp.Tests
{
    [TestClass]
    public class CarStatServiceTests
    {
        private CarStatsDbContext _context;
        private List<CarStat> _carStatsList;
        private string _databaseName;
        private Mock<IUploadFileService> _mockUploadService;
        private CarStatService _service;

        [TestInitialize]
        public void Initialize()
        {
            _databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<CarStatsDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new CarStatsDbContext(options);
            _mockUploadService = new Mock<IUploadFileService>();

            _carStatsList = new List<CarStat>
            {
                new CarStat { Id = 1, Date = new DateTime(2023, 10, 1, 0, 0, 0), Speed = 50, RegistrationNumber = "ABC123" },
                new CarStat { Id = 2, Date = new DateTime(2023, 10, 1, 0, 0, 0), Speed = 70, RegistrationNumber = "DEF123" },
                new CarStat { Id = 3, Date = new DateTime(2023, 8, 1, 0, 0, 0), Speed = 70, RegistrationNumber = "IJK123" },
                new CarStat { Id = 4, Date = new DateTime(2023, 8, 1, 0, 0, 0), Speed = 60, RegistrationNumber = "LMN123" },
                new CarStat { Id = 5, Date = new DateTime(2023, 7, 1, 0, 0, 0), Speed = 100, RegistrationNumber = "OPR123" },
                new CarStat { Id = 6, Date = new DateTime(2023, 9, 1, 1, 0, 0), Speed = 100, RegistrationNumber = "OPR123" },
                new CarStat { Id = 7, Date = new DateTime(2023, 9, 1, 1, 0, 0), Speed = 50, RegistrationNumber = "OPR123" },
                new CarStat { Id = 8, Date = new DateTime(2023, 9, 1, 2, 0, 0), Speed = 50, RegistrationNumber = "OPR123" },
                new CarStat { Id = 9, Date = new DateTime(2023, 9, 1, 2, 0, 0), Speed = 60, RegistrationNumber = "OPR123" },
            };

            _context.CarStats.AddRange(_carStatsList);
            _context.SaveChanges();

            _service = new CarStatService(_context, _mockUploadService.Object);
        }

        [TestMethod]
        public async Task GetFilteredCarStatsAsync_WithoutSpeedAndDateValues_ReturnsAllRecords()
        {
            var (result, totalRecords) = await _service.GetFilteredCarStatsAsync(1, null, null, null, null, CancellationToken.None);

            var resultList = result.ToList();

            totalRecords.Should().Be(_carStatsList.Count);            
            resultList[0].Id.Should().Be(_carStatsList[0].Id);
            resultList[1].Id.Should().Be(_carStatsList[1].Id);
            resultList[2].Id.Should().Be(_carStatsList[2].Id);
            resultList[3].Id.Should().Be(_carStatsList[3].Id);
            resultList[4].Id.Should().Be(_carStatsList[4].Id);
        }

        [TestMethod]
        public async Task GetFilteredCarStatsAsync_WithSpeedFrom_ReturnsExpectedRecords()
        {
            var (result, totalRecords) = await _service.GetFilteredCarStatsAsync(1, 70, null, null, null, CancellationToken.None);

            var resultList = result.ToList();

            totalRecords.Should().Be(4);
            resultList[0].Id.Should().Be(2);
            resultList[1].Id.Should().Be(3);
            resultList[2].Id.Should().Be(5);
        }

        [TestMethod]
        public async Task GetFilteredCarStatsAsync_WithSpeedFromAndTo_ReturnsExpectedRecords()
        {
            var (result, totalRecords) = await _service.GetFilteredCarStatsAsync(1, 50, 60, null, null, CancellationToken.None);

            var resultList = result.ToList();

            totalRecords.Should().Be(5);
            resultList[0].Id.Should().Be(1);
            resultList[1].Id.Should().Be(4);
        }

        [TestMethod]
        public async Task GetFilteredCarStatsAsync_WithDateFrom_ReturnsExpectedRecords()
        {
            var (result, totalRecords) = await _service.GetFilteredCarStatsAsync(1, null, null, new DateTime(2023, 8, 15), null, CancellationToken.None);

            var resultList = result.ToList();

            totalRecords.Should().Be(6);
            resultList[0].Id.Should().Be(1);
            resultList[1].Id.Should().Be(2);
        }

        [TestMethod]
        public async Task GetFilteredCarStatsAsync_WithDateFromAndTo_ReturnsExpectedRecords()
        {
            var (result, totalRecords) = await _service.GetFilteredCarStatsAsync(1, null, null, new DateTime(2023, 8, 1), new DateTime(2023, 10, 15), CancellationToken.None);

            var resultList = result.ToList();

            totalRecords.Should().Be(8);            
            resultList[0].Id.Should().Be(1);
            resultList[1].Id.Should().Be(2);
            resultList[2].Id.Should().Be(3);
            resultList[3].Id.Should().Be(4);
        }

        [TestMethod]
        public async Task GetFilteredCarStatsAsync_WithAllFilterValues_ReturnsExpectedRecords()
        {
            var (result, totalRecords) = await _service.GetFilteredCarStatsAsync(1, 60, 70, new DateTime(2023, 8, 1), new DateTime(2023, 10, 15), CancellationToken.None);

            var resultList = result.ToList();

            totalRecords.Should().Be(4);            
            resultList[0].Id.Should().Be(2);
            resultList[1].Id.Should().Be(3);
            resultList[2].Id.Should().Be(4);
        }

        [TestMethod]
        public async Task GetAverageSpeedsByDayAsync_ReturnsCorrectHourlyAverages()
        {
            var result = await _service.GetAverageSpeedsByDayAsync(new DateTime(2023, 9, 1));

            result.Should().NotBeNull();
            result.Should().BeOfType<List<HourlyAverageSpeed>>();
            result.Should().HaveCount(2);
            result[0].AverageSpeed.Should().Be(75);
            result[0].Hour.Should().Be(1);
            result[1].AverageSpeed.Should().Be(55);
            result[1].Hour.Should().Be(2);

        }

        [TestMethod]
        public async Task AddRangeAsync_AddsDataSuccessfully()
        {
            var newData = new List<CarStat>
            {
                new CarStat { Date = new DateTime(2024, 1, 1), Speed = 80, RegistrationNumber = "GHI789" },
                new CarStat { Date = new DateTime(2024, 1, 2), Speed = 90, RegistrationNumber = "JKL012" }
            };

            var oldDataCount = _context.CarStats.Count();

            var options = new DbContextOptionsBuilder<CarStatsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockFile = new Mock<IFormFile>();
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes("fake file content"));
            mockFile.Setup(m => m.OpenReadStream()).Returns(sourceStream);
            mockFile.Setup(m => m.FileName).Returns("test.txt");
            mockFile.Setup(m => m.Length).Returns(sourceStream.Length);

            _mockUploadService.Setup(m => m.ParseFileAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(newData);

            await _service.AddRangeAsync(mockFile.Object, CancellationToken.None);

            var allData = await _context.CarStats.ToListAsync();

            allData.Should().HaveCount(newData.Count + oldDataCount);
            foreach (var carStat in newData)
            {
                allData.Should().Contain(cs =>
                    cs.Date == carStat.Date &&
                    cs.Speed == carStat.Speed &&
                    cs.RegistrationNumber == carStat.RegistrationNumber);
            }
        }


        [TestMethod]
        public async Task ClearAllAsync_RemovesAllData()
        {
            var options = new DbContextOptionsBuilder<CarStatsDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            using (var arrangeContext = new CarStatsDbContext(options))
            {
                Assert.AreEqual(_carStatsList.Count, await arrangeContext.CarStats.CountAsync());
            }

            await _service.ClearAllAsync(CancellationToken.None);

            using (var assertContext = new CarStatsDbContext(options))
            {
                var carStatsCount = await assertContext.CarStats.CountAsync();
                carStatsCount.Should().Be(0);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}