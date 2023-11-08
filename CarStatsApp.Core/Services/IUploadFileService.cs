using CarStatsApp.Core.Models;

namespace CarStatsApp.Core.Services
{
    public interface IUploadFileService
    {
        Task<List<CarStat>> ParseFileAsync(Stream fileStream, CancellationToken cancellationToken = default);
    }
}