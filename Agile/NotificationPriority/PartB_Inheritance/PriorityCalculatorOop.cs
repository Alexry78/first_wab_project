using System;

namespace NotificationPriority.PartB_Inheritance  
{

    public class PriorityCalculatorOop
    {

        /// <param name="eventObj">Объект события</param>
        /// <param name="context">Контекст уведомления</param>
        /// <returns>Итоговый приоритет</returns>
        public int CalculatePriority(Event eventObj, NotifyContext context)
        {
            if (eventObj == null)
                throw new ArgumentNullException(nameof(eventObj));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Просто вызываем виртуальный метод
            return eventObj.GetPriority(context);
        }

        public void Demonstrate(Event eventObj, NotifyContext context)
        {
            Console.WriteLine($"\nТип события: {eventObj.GetType().Name}");
            Console.WriteLine(context);
            
            int result = CalculatePriority(eventObj, context);
            
            Console.WriteLine($"ИТОГОВЫЙ ПРИОРИТЕТ: {result}");
        }
    }
}