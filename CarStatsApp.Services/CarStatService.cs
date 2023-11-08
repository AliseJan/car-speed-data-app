using CarStatsApp.Core.Models;
using CarStatsApp.Core.Services;
using CarStatsApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarStatsApp.Services
{
    public class CarStatService : ICarStatService
    {        
        private readonly ICarStatsDbContext _context;
        private readonly IUploadFileService _service;
        private const int _RecordsPerPage = 20;

        public CarStatService(ICarStatsDbContext context, IUploadFileService service)
        {
            _context = context;
            _service = service;
        }

        public int GetRecordsPerPage() => _RecordsPerPage;

        public async Task<(List<CarStat>, int)> GetFilteredCarStatsAsync(
        int pageNumber,
        int? speedFrom, int? speedTo,
        DateTime? dateFrom, DateTime? dateTo,
        CancellationToken cancellationToken = default)
        {
            var query = _context.CarStats.AsQueryable();

            if (speedFrom.HasValue)
            {
                query = query.Where(cs => cs.Speed >= speedFrom.Value);
            }                

            if (speedTo.HasValue)
            {
                query = query.Where(cs => cs.Speed <= speedTo.Value);
            }                

            if (dateFrom.HasValue)
            {
                query = query.Where(cs => cs.Date >= dateFrom.Value);
            }
                
            if (dateTo.HasValue)
            {
                var endDate = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(cs => cs.Date <= endDate);
            }

            int totalRecords = await query.CountAsync(cancellationToken);
            List<CarStat> records = await query
                .OrderBy(cs => cs.Id)
                .Skip((pageNumber - 1) * _RecordsPerPage)
                .Take(_RecordsPerPage)
                .ToListAsync(cancellationToken);

            return (records, totalRecords);
        }

        public async Task<List<HourlyAverageSpeed>> GetAverageSpeedsByDayAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var averageSpeeds = await _context.CarStats
                .Where(cs => cs.Date >= startDate && cs.Date < endDate)
                .GroupBy(cs => cs.Date.Hour)
                .Select(g => new HourlyAverageSpeed
                {
                    Hour = g.Key,
                    AverageSpeed = g.Average(cs => cs.Speed)
                })
                .OrderBy(ha => ha.Hour)
                .ToListAsync(cancellationToken);

            return averageSpeeds;
        }

        public async Task AddRangeAsync([FromForm] IFormFile file, CancellationToken cancellationToken = default)
        {
            using var stream = file.OpenReadStream();
            var carStats = await _service.ParseFileAsync(stream);

            await _context.CarStats.AddRangeAsync(carStats, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ClearAllAsync(CancellationToken cancellationToken = default)
        {
            _context.CarStats.RemoveRange(_context.CarStats);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}