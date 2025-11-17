using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MES.Data.Enums;

namespace MES.Data.Entities
{
    public class Order : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrderID { get; set; }

        // Связь с планом
        public int ProductionPlanID { get; set; }

        [ForeignKey(nameof(ProductionPlanID))]
        public ProductionPlan Plan { get; set; } = null!;

        // Основные данные
        public int Quantity { get; set; }

        public OrderPriority Priority { get; set; } = OrderPriority.Medium;

        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public DateTime Deadline { get; set; }
        
        // Добавляем поля для аутентификации и аудита
        public int? AssignedOperatorId { get; set; }

        [ForeignKey(nameof(AssignedOperatorId))]
        public User? AssignedOperator { get; set; }

        public int CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public User CreatedByUser { get; set; } = null!;

        public DateTime? StartedAt { get; set; }
        public int? StartedBy { get; set; }

        public DateTime? CompletedAt { get; set; }
        public int? CompletedBy { get; set; }

        // Вычисляемое свойство (не маппится в БД)
        [NotMapped]
        public bool IsOverdue => Deadline < DateTime.UtcNow && Status != OrderStatus.Completed;
    }
}