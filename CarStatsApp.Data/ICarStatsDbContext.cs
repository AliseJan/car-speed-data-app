using CarStatsApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CarStatsApp.Data;

public interface ICarStatsDbContext
{
    DbSet<CarStat> CarStats { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}