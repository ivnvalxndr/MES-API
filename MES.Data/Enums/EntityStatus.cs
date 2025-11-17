namespace MES.Data.Enums;

public enum EntityStatus
{
    Draft = 1,      // Черновик
    Planned = 2,    // Запланирован
    Active = 3,     // Активен
    InProgress = 4, // В работе
    Completed = 5,  // Завершен
    Cancelled = 6,  // Отменен
    OnHold = 7,     // На паузе
    Archived = 8    // В архиве
}