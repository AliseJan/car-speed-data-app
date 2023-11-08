using CarStatsApp.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace CarStatAppTests
{
    [TestClass]
    public class UploadServiceTests
    {
        private Mock<ILogger<UploadFileService>> _mockLogger;
        private UploadFileService _service;

        [TestInitialize]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<UploadFileService>>();
            _service = new UploadFileService(_mockLogger.Object);
        }

        [TestMethod]
        public async Task ParseFileAsync_WithCorrectData_CorrectlyParsesCarStats()
        {
            var fileContent = "2023-01-01 00:00:00\t120\tABC123\r\n2023-01-02 00:00:00\t100\tDEF456\r\n";
            using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var result = await _service.ParseFileAsync(fileStream);

            result.Should().HaveCount(2);
            result[0].Date.Should().Be(new DateTime(2023, 1, 1));
            result[0].Speed.Should().Be(120f);
            result[0].RegistrationNumber.Should().Be("ABC123");
        }

        [TestMethod]
        public async Task ParseFileAsync_WithMissingEntryData_SkipsIncorrectlyFormattedLinesAndLogsWarning()
        {
            var fileContent = "2023-01-01 00:00:00\t120\r\n2023-01-02 00:00:00\t100\tDEF456\r\n";
            using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var result = await _service.ParseFileAsync(fileStream);

            result.Should().HaveCount(1);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().StartsWith("Line 1 skipped, incorrect number of data parts:")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ParseFileAsync_WithIncorrectEntry_HandlesFormatExceptionAndLogsError()
        {
            var fileContent = "BadDateFormat\tNotANumber\tXYZ789\r\n2023-01-02 00:00:00\t100\tDEF456\r\n";
            using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var result = await _service.ParseFileAsync(fileStream);

            result.Should().HaveCount(1);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().StartsWith("FormatException on line 1:")),
                    It.IsAny<FormatException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}