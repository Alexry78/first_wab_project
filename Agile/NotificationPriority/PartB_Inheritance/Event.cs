using System;

namespace NotificationPriority.PartB_Inheritance
{
    public abstract class Event
    {
        private readonly int _basePriority;

       /// <param name="basePriority">Базовый приоритет события</param>
        protected Event(int basePriority)
        {
            _basePriority = basePriority;
        }

        public int BasePriority => _basePriority;

       /// <param name="context">Контекст уведомления</param>
        /// <returns>Итоговый приоритет</returns>
        public virtual int GetPriority(NotifyContext context)
        {
            return BasePriority;
        }

       protected int ApplyModifiers(int priority, NotifyContext context, bool ignoreModifiers = false)
        {
            if (ignoreModifiers)
                return priority;

            Console.WriteLine($"  Базовый приоритет: {priority}");

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
    }
}