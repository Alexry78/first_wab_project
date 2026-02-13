using System;

namespace RestaurantProject
{
    public interface IOrder
    {
 
        string Customer { get; set; }
  
        int TotalCents { get; set; }

        /// <param name="percent">Процент скидки</param>
        void ApplyDiscount(int percent);
    }
    public static class OrderExtensions
    {
        public static void DefaultApplyDiscount(this IOrder order, int percent)
        {
            if (percent < 0 || percent > 100)
            {
                Console.WriteLine("Ошибка: процент скидки должен быть от 0 до 100");
                return;
            }

            int discountAmount = order.TotalCents * percent / 100;
            order.TotalCents = Math.Max(0, order.TotalCents - discountAmount);
            Console.WriteLine($"Применена скидка {percent}%. Новая сумма: {order.TotalCents / 100.0:F2} руб.");
        }
    }
}