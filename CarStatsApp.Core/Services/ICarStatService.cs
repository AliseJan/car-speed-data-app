using CarStatsApp.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarStatsApp.Core.Services;

public interface ICarStatService
{
    int GetRecordsPerPage();
    Task<(List<CarStat>, int)> GetFilteredCarStatsAsync(
    int pageNumber,
    int? speedFrom, int? speedTo,
    DateTime? dateFrom, DateTime? dateTo,
    CancellationToken cancellationToken = default);
    Task<List<HourlyAverageSpeed>> GetAverageSpeedsByDayAsync(DateTime date, CancellationToken cancellationToken = default);
    Task AddRangeAsync([FromForm] IFormFile file, CancellationToken cancellationToken = default);
    Task ClearAllAsync(CancellationToken cancellationToken = default);
}