using System;
using System.Collections.Generic;

namespace RestaurantProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА УПРАВЛЕНИЯ ЗАКАЗАМИ В РЕСТОРАНЕ ===\n");

          
            SimpleOrder simpleOrder = new SimpleOrder("Иван Петров", 3500); // 3500 центов = 35.00 руб.

            
            MemberOrder memberOrder = new MemberOrder("Анна Сидорова", 4500, 2000); // 45.00 руб., мин. чек 20.00 руб.

            Console.WriteLine("=== ТЕСТ 1: Проверка интерфейсов и свойств ===");
            Console.WriteLine(simpleOrder);
            Console.WriteLine(memberOrder);

            Console.WriteLine("\n=== ТЕСТ 2: Применение скидок к SimpleOrder (дефолтная реализация) ===");
            Console.WriteLine($"Начальная сумма: {simpleOrder.TotalCents / 100.0:F2} руб.");
            simpleOrder.ApplyDiscount(10); // Скидка 10%
            Console.WriteLine($"После скидки: {simpleOrder.TotalCents / 100.0:F2} руб.");

            Console.WriteLine("\n=== ТЕСТ 3: Применение скидок к MemberOrder с защитой минимального чека ===");
            Console.WriteLine(memberOrder);
            Console.WriteLine($"Начальная сумма: {memberOrder.TotalCents / 100.0:F2} руб.");
            Console.WriteLine($"Минимальный чек: {memberOrder.MinBillCents / 100.0:F2} руб.");

            Console.WriteLine("\n--- Скидка 20% (безопасная) ---");
            memberOrder.ApplyDiscount(20); 

            Console.WriteLine("\n--- Скидка 70% (должна ограничиться минимальным чеком) ---");
            memberOrder.ApplyDiscount(70); 

            Console.WriteLine("\n=== ТЕСТ 4: Начисление бонусных баллов ===");
            Console.WriteLine("Создаем новый заказ участника с начальными баллами:");
            MemberOrder newMember = new MemberOrder("Сергей Козлов", 5000, 1500);
            Console.WriteLine(newMember);

            Console.WriteLine("\n--- Добавляем баллы через AddPoints ---");
            newMember.AddPoints(100);
            newMember.AddPoints(50);

            Console.WriteLine("\n--- Применяем скидку (должна начислить бонусные баллы) ---");
            newMember.ApplyDiscount(15); 
            Console.WriteLine("\n=== ТЕСТ 5: Дополнительный функционал - оплата баллами ===");
            Console.WriteLine($"Текущий заказ: {newMember.TotalCents / 100.0:F2} руб., баллы: {newMember.LoyaltyPoints}");

            Console.WriteLine("\n--- Попытка оплатить 200 баллов ---");
            newMember.PayWithPoints(200);

            Console.WriteLine("\n--- Попытка оплатить 30 баллов ---");
            newMember.PayWithPoints(30);

            Console.WriteLine("\n=== ТЕСТ 6: Сравнение SimpleOrder и MemberOrder ===");
            
            
            List<IOrder> orders = new List<IOrder>
            {
                new SimpleOrder("Клиент 1", 2800),
                new SimpleOrder("Клиент 2", 3900),
                new MemberOrder("Клиент 3", 5200, 2000),
                new MemberOrder("Клиент 4", 4300, 1800)
            };

            Console.WriteLine("\nВсе заказы до скидок:");
            foreach (var order in orders)
            {
                Console.WriteLine($"  {order}");
            }

            Console.WriteLine("\nПрименяем скидку 25% ко всем заказам:");
            foreach (var order in orders)
            {
                Console.WriteLine($"\n{order.Customer}:");
                order.ApplyDiscount(25);
            }

            Console.WriteLine("\n=== ТЕСТ 7: Проверка валидации ===");
            
            try
            {
                Console.WriteLine("Попытка создать заказ с пустым именем:");
                SimpleOrder badOrder = new SimpleOrder("", 1000);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            try
            {
                Console.WriteLine("\nПопытка создать заказ с отрицательной суммой:");
                SimpleOrder badOrder = new SimpleOrder("Тест", -500);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            try
            {
                Console.WriteLine("\nПопытка применить некорректную скидку:");
                memberOrder.ApplyDiscount(150);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\n=== ИТОГОВОЕ СОСТОЯНИЕ ===");
            Console.WriteLine(simpleOrder);
            Console.WriteLine(memberOrder);
            Console.WriteLine(newMember);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}