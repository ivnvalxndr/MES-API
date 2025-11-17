using MES.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MES.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    // Ваши DbSet (Users теперь не нужен - есть в Identity)
    public DbSet<ProductionPlan> ProductionPlans { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Важно вызвать базовый!

        // Уберите конфигурацию для User - Identity сделает это автоматически

        // Конфигурация для Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderID);

            entity.HasOne(o => o.Plan)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductionPlanID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.AssignedOperator)
                .WithMany(u => u.AssignedOrders)
                .HasForeignKey(o => o.AssignedOperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(o => o.CreatedByUser)
                .WithMany(u => u.CreatedOrders)
                .HasForeignKey(o => o.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(o => o.Priority).HasConversion<string>();
            entity.Property(o => o.Status).HasConversion<string>();
        });

        // Конфигурация для ProductionPlan
        modelBuilder.Entity<ProductionPlan>(entity =>
        {
            entity.HasKey(p => p.Id);
        });
    }
}