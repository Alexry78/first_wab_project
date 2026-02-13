using System;

namespace NotificationPriority.PartA_EnumSwitch
{
    public class PriorityCalculatorEnum
    {
        
        /// <param name="eventType">Тип события</param>
        /// <param name="context">Контекст уведомления</param>
        /// <returns>Итоговый приоритет</returns>
        public int CalculatePriority(EventType eventType, NotifyContext context)
        {
            int basePriority;
            
            switch (eventType)
            {
                case EventType.Info:
                    basePriority = (int)EventType.Info;
                    break;
                case EventType.Warning:
                    basePriority = (int)EventType.Warning;
                    break;
                case EventType.Alert:
                    basePriority = (int)EventType.Alert;
                    break;
                case EventType.Critical:
                    basePriority = (int)EventType.Critical;
                    break;
                case EventType.SystemHeartbeat:
                    return 1;
                default:
                    throw new ArgumentException($"Неизвестный тип события: {eventType}");
            }

            int priority = basePriority;

            if (context.IsQuietHours)
            {
                priority -= 2;
                priority = Math.Max(1, priority);
                Console.WriteLine("  Применен модификатор тихого часа: -2");
            }

               if (context.IsVIPUser)
            {
                priority += 2;
                Console.WriteLine("  Применен модификатор VIP: +2");
            }

             if (context.HasCriticalFlag)
            {
                priority += 3;
                Console.WriteLine("  Применен модификатор критического флага: +3");
            }

            return priority;
        }


        public void Demonstrate(EventType eventType, NotifyContext context)
        {
            Console.WriteLine($"\nТип события: {eventType}");
            Console.WriteLine(context);
            
            int result = CalculatePriority(eventType, context);
            
            Console.WriteLine($"ИТОГОВЫЙ ПРИОРИТЕТ: {result}");
        }
    }
}