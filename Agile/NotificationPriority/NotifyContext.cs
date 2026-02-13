namespace NotificationPriority
{
    public class NotifyContext
    {
        public bool IsQuietHours { get; set; }

        public bool IsVIPUser { get; set; }

        public bool HasCriticalFlag { get; set; }

        public NotifyContext(bool isQuietHours = false, bool isVIPUser = false, bool hasCriticalFlag = false)
        {
            IsQuietHours = isQuietHours;
            IsVIPUser = isVIPUser;
            HasCriticalFlag = hasCriticalFlag;
        }

        public override string ToString()
        {
            return $"Контекст: [Тихий час: {(IsQuietHours ? "Да" : "Нет")}, " +
                   $"VIP: {(IsVIPUser ? "Да" : "Нет")}, " +
                   $"Крит.флаг: {(HasCriticalFlag ? "Да" : "Нет")}]";
        }
    }
}