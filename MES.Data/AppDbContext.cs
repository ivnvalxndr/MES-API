using MES.Data.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace MES.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    // Добавляем DbSet для ваших сущностей
    public DbSet<ProductionPlan> ProductionPlans { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Equipment> Equipment { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Здесь можно настроить модели (опционально)
        modelBuilder.Entity<ProductionPlan>().HasKey(p => p.Id);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
    }
}