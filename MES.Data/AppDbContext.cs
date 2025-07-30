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
        // Для всех сущностей
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Таблица в UPPER CASE
            entity.SetTableName(entity.GetTableName().ToUpper());

            // Колонки
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnBaseName().ToUpper());
            }

            // Первичный ключ (исправленная версия)
            if (entity.FindPrimaryKey() is { } primaryKey)
            {
                primaryKey.SetName($"PK_{entity.GetTableName()}".ToUpper());
            }

            // Внешние ключи (исправленная версия)
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
    }
}