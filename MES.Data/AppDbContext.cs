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
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Для всех сущностей
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Таблица в UPPER CASE
            entity.SetTableName(entity.GetTableName()?.ToUpper());

            // Колонки
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToUpper());
            }

            // Первичный ключ
            if (entity.FindPrimaryKey() is { } primaryKey)
            {
                primaryKey.SetName($"PK_{entity.GetTableName()}".ToUpper());
            }

            // Внешние ключи
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(
                    $"FK_{entity.GetTableName()}_{foreignKey.PrincipalEntityType.GetTableName()}".ToUpper());
            }

            // Индексы
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName($"IX_{entity.GetTableName()}_{string.Join("_", index.Properties.Select(p => p.Name))}".ToUpper());
            }
        }

        /*// Здесь можно настроить модели (опционально)
        modelBuilder.Entity<ProductionPlan>().HasKey(p => p.Id);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);*/

        // Конкретные настройки для ProductionPlan
        /*modelBuilder.Entity<ProductionPlan>(entity =>
        {
            // Настройка связи с Order
            entity.HasOne(p => p.Order)
                .WithMany(o => o.ProductionPlans)
                .HasForeignKey("ORDERID")  // Явное указание имени FK
                .HasConstraintName("FK_PRODUCTIONPLAN_ORDER");
        });*/

        // Конфигурация для User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Role).HasDefaultValue("Operator");
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
        });

        // Начальные данные
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "admin123", // Замените на хеш!
                Role = UserRole.Admin,
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "operator1",
                PasswordHash = "operator123",
                Role = UserRole.Operator,
                IsActive = true
            },
            new User
            {
                Id = 3,
                Username = "technologist1",
                PasswordHash = "tech123",
                Role = UserRole.Technologist,
                IsActive = true
            },
            new User
            {
                Id = 4,
                Username = "manager1",
                PasswordHash = "manager123",
                Role = UserRole.Manager,
                IsActive = true
            }
        );
    }
}