using CarStatsApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CarStatsApp.Data;

public class CarStatsDbContext : DbContext, ICarStatsDbContext
{
    public CarStatsDbContext(DbContextOptions<CarStatsDbContext> options) 
        : base(options) 
    { 
    }

    public DbSet<CarStat> CarStats { get; set; }
}