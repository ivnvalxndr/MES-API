namespace MES.Data.Enums;

public enum OrderStatus
{
    Draft = 1,          // Черновик
    Planned = 2,        // Запланирован
    InProgress = 3,     // В работе
    Completed = 4,      // Завершен
    Cancelled = 5,      // Отменен
    OnHold = 6          // На паузе
}