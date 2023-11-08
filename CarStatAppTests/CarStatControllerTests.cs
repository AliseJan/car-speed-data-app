using CarStatsApp.Controllers;
using CarStatsApp.Core.Models;
using CarStatsApp.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace CarStatAppTests
{
    [TestClass]
    public class CarStatsControllerTests
    {
        private Mock<ICarStatService> _mockService;
        private Mock<IUploadFileService> _mockUploadFileService;
        private CarStatsController _controller;
        private List<CarStat> _carStatsList;

        [TestInitialize]
        public void Initialize()
        {
            _mockService = new Mock<ICarStatService>();
            _mockUploadFileService = new Mock<IUploadFileService>();

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

            _mockService.Setup(s => s.GetFilteredCarStatsAsync(
                    It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<CarStat>(_carStatsList), _carStatsList.Count));

            _mockService.Setup(s => s.GetRecordsPerPage()).Returns(20);

            _controller = new CarStatsController(_mockService.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TestMethod]
        public async Task GetCarStats_WithCorrectData_ReturnsOkStatus()
        {
            int expectedPageNumber = 1;
            int expectedTotalRecords = _carStatsList.Count;
            int recordsPerPage = 20;
            int expectedTotalPages = (int)Math.Ceiling((double)expectedTotalRecords / recordsPerPage);

            var result = await _controller.GetCarStats(expectedPageNumber);

            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(new
            {
                Data = _carStatsList,
                TotalPages = expectedTotalPages,
                CurrentPage = expectedPageNumber,
                RecordsPerPage = recordsPerPage,
                TotalRecords = expectedTotalRecords
            });
        }

        [TestMethod]
        public async Task Upload_ValidFile_ReturnsOkStatus()
        {
            var mockFile = new Mock<IFormFile>();
            var fileName = "test.csv";
            var content = "2023-01-01 00:00:00\t120\tABC123\n";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var writer = new StreamWriter(ms);
            await writer.FlushAsync();
            ms.Position = 0;

            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            _mockUploadFileService.Setup(u => u.ParseFileAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new List<CarStat>());
            _mockService.Setup(s => s.AddRangeAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

            var result = await _controller.Upload(mockFile.Object);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("Data successfully uploaded and parsed.");
        }

        [TestMethod]
        public async Task Upload_InvalidFile_ReturnsBadRequestStatus()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(_ => _.Length).Returns(0);

            var result = await _controller.Upload(mockFile.Object);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("File is empty.");
        }

        [TestMethod]
        public async Task Upload_OperationCancelled_ReturnsStatusCode499()
        {
            var mockFile = new Mock<IFormFile>();
            var content = "some content";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestAborted = cancellationTokenSource.Token
                }
            };

            _mockService.Setup(s => s.AddRangeAsync(It.IsAny<IFormFile>(), cancellationTokenSource.Token))
                .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            var result = await _controller.Upload(mockFile.Object);

            result.Should().BeOfType<ObjectResult>();
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.StatusCode.Should().Be(499);
            statusCodeResult.Value.Should().Be("Operation cancelled by the user.");
        }


        [TestMethod]
        public async Task Upload_ReturnsInternalServerErrorOnException()
        {
            var mockFile = new Mock<IFormFile>();
            var content = "some content";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var exceptionMessage = "An error occurred.";
            _mockService.Setup(s => s.AddRangeAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            var result = await _controller.Upload(mockFile.Object);

            result.Should().BeOfType<ObjectResult>();
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().Be($"Internal server error: {exceptionMessage}");
        }

        [TestMethod]
        public async Task ClearData_ReturnsOkWhenSuccessful()
        {
            _mockService.Setup(s => s.ClearAllAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.ClearData();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("All records have been deleted.");
        }

        [TestMethod]
        public async Task ClearData_ReturnsInternalServerErrorOnException()
        {
            var exception = new Exception("An error occurred.");
            _mockService.Setup(s => s.ClearAllAsync(It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            var result = await _controller.ClearData();

            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be($"Internal server error: {exception}");
        }

        [TestMethod]
        public async Task GetAverageSpeedByDay_ReturnsOkWithAverageSpeedData()
        {
            var testDate = new DateTime(2023, 1, 1);
            var testData = new List<HourlyAverageSpeed>
            {
                new HourlyAverageSpeed { Hour = 10, AverageSpeed = 55.0 },
                new HourlyAverageSpeed { Hour = 11, AverageSpeed = 65.0 },
            };
            _mockService.Setup(s => s.GetAverageSpeedsByDayAsync(testDate, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(testData);

            var result = await _controller.GetAverageSpeedByDay(testDate);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(testData, options => options.ComparingByMembers<HourlyAverageSpeed>());
        }

        [TestMethod]
        public async Task GetAverageSpeedByDay_NoDataAvailable_ReturnsNotFound()
        {
            var testDate = new DateTime(2023, 1, 1);
            _mockService.Setup(s => s.GetAverageSpeedsByDayAsync(testDate, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<HourlyAverageSpeed>());

            var result = await _controller.GetAverageSpeedByDay(testDate);

            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.Value.Should().Be("No data available for the selected date.");
        }

        [TestMethod]
        public async Task GetAverageSpeedByDay_ReturnsInternalServerErrorOnException()
        {
            var testDate = new DateTime(2023, 1, 1);
            var expectedException = new Exception("An error occurred.");
            _mockService.Setup(s => s.GetAverageSpeedsByDayAsync(testDate, It.IsAny<CancellationToken>()))
                        .ThrowsAsync(expectedException);

            var result = await _controller.GetAverageSpeedByDay(testDate);

            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be($"Internal server error: {expectedException}");
        }
    }
}