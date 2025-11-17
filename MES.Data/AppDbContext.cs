using MES.Data.Entities;
using MES.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    // Добавляем DbSet 
    public DbSet<ProductionPlan> ProductionPlans { get; set; }
    public DbSet<Order> Orders { get; set; }
    //public DbSet<Equipment> Equipment { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация для Order - исправляем связи
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderID);

            // Связь с ProductionPlan - убеждаемся что типы совместимы
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

            // Конвертация Enum
            entity.Property(o => o.Priority).HasConversion<string>();
            entity.Property(o => o.Status).HasConversion<string>();
        });

        // Конфигурация для ProductionPlan
        modelBuilder.Entity<ProductionPlan>(entity =>
        {
            entity.HasKey(p => p.Id); // Должен быть тот же тип что и в Order.ProductionPlanID
        });

        // Конфигурация для User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
        });

        var staticDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // Начальные данные
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "admin123",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = staticDate 
            },
            new User
            {
                Id = 2,
                Username = "operator1",
                PasswordHash = "operator123",
                Role = UserRole.Operator,
                IsActive = true,
                CreatedAt = staticDate
            },
            new User
            {
                Id = 3,
                Username = "technologist1",
                PasswordHash = "tech123",
                Role = UserRole.Technologist,
                IsActive = true,
                CreatedAt = staticDate
            },
            new User
            {
                Id = 4,
                Username = "manager1",
                PasswordHash = "manager123",
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = staticDate
            }
        );
    }
}