using CarStatsApp.Core.Models;
using CarStatsApp.Core.Services;
using Microsoft.Extensions.Logging;

namespace CarStatsApp.Services
{
    public class UploadFileService: IUploadFileService
    {
        private readonly ILogger<UploadFileService> _logger;

        public UploadFileService(ILogger<UploadFileService> logger)
        {
            _logger = logger;
        }

        public async Task<List<CarStat>> ParseFileAsync(Stream fileStream, CancellationToken cancellationToken = default)
        {
            var carStats = new List<CarStat>();
            using var reader = new StreamReader(fileStream);

            var content = await reader.ReadToEndAsync();
            var lines = content.Split(Environment.NewLine);

            int lineNumber = 0;

            foreach (var line in lines)
            {
                cancellationToken.ThrowIfCancellationRequested();
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('\t');

                if (parts.Length == 3)
                {
                    try
                    {
                        var carStat = new CarStat
                        {
                            Date = DateTime.Parse(parts[0].Trim()),
                            Speed = float.Parse(parts[1].Trim()),
                            RegistrationNumber = parts[2].Trim()
                        };

                        carStats.Add(carStat);
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogError(ex, $"FormatException on line {lineNumber}: {line}");
                        continue;
                    }
                }
                else
                {
                    _logger.LogWarning($"Line {lineNumber} skipped, incorrect number of data parts: {line}");
                }
            }

            return carStats;
        }
    }
}