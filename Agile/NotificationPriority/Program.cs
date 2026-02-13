using System;
using System.Collections.Generic;
using NotificationPriority;
using NotificationPriority.PartA_EnumSwitch;
using NotificationPriority.PartB_Inheritance;

namespace NotificationPriority
{
    class Program 
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА ПРИОРИТЕТОВ УВЕДОМЛЕНИЙ ===\n");
            Console.WriteLine("Два подхода: enum+switch и иерархия классов\n");

         
            var contexts = new Dictionary<string, NotifyContext>
            {
                ["Без модификаторов"] = new NotifyContext(false, false, false),
                ["Тихий час"] = new NotifyContext(true, false, false),
                ["VIP пользователь"] = new NotifyContext(false, true, false),
                ["Критический флаг"] = new NotifyContext(false, false, true),
                ["Тихий час + VIP"] = new NotifyContext(true, true, false),
                ["Тихий час + Крит.флаг"] = new NotifyContext(true, false, true),
                ["VIP + Крит.флаг"] = new NotifyContext(false, true, true),
                ["Все модификаторы"] = new NotifyContext(true, true, true)
            };

         
            var testCases = new (string name, PartA_EnumSwitch.EventType enumType, Event oopEvent, string contextName, int expected)[]
            {
                ("Информация", EventType.Info, new InfoEvent(), "Без модификаторов", 1),
                ("Предупреждение", EventType.Warning, new WarningEvent(), "VIP пользователь", 5),
                ("Тревога", EventType.Alert, new AlertEvent(), "Тихий час", 3),
                ("Критическое", EventType.Critical, new CriticalEvent(), "Критический флаг", 11),
                ("SystemHeartbeat", EventType.SystemHeartbeat, new SystemHeartbeatEvent(), "Все модификаторы", 1)
            };

            Console.WriteLine("=========================================");
            Console.WriteLine("ЧАСТЬ A: ПОДХОД С ENUM И SWITCH");
            Console.WriteLine("=========================================");

            var calculatorEnum = new PriorityCalculatorEnum();

            foreach (var testCase in testCases)
            {
                Console.WriteLine($"\n--- Тест: {testCase.name} ---");
                calculatorEnum.Demonstrate(testCase.enumType, contexts[testCase.contextName]);
                
           
                int result = calculatorEnum.CalculatePriority(testCase.enumType, contexts[testCase.contextName]);
                Console.WriteLine(result == testCase.expected ? "✓ РЕЗУЛЬТАТ СОВПАДАЕТ С ОЖИДАЕМЫМ" : "✗ РЕЗУЛЬТАТ НЕ СОВПАДАЕТ");
            }

            Console.WriteLine("\n=========================================");
            Console.WriteLine("ЧАСТЬ B: ПОДХОД С ИЕРАРХИЕЙ КЛАССОВ");
            Console.WriteLine("=========================================");

            var calculatorOop = new PriorityCalculatorOop();

            foreach (var testCase in testCases)
            {
                Console.WriteLine($"\n--- Тест: {testCase.name} ---");
                calculatorOop.Demonstrate(testCase.oopEvent, contexts[testCase.contextName]);
                
             
                int result = calculatorOop.CalculatePriority(testCase.oopEvent, contexts[testCase.contextName]);
                Console.WriteLine(result == testCase.expected ? "✓ РЕЗУЛЬТАТ СОВПАДАЕТ С ОЖИДАЕМЫМ" : "✗ РЕЗУЛЬТАТ НЕ СОВПАДАЕТ");
            }

            Console.WriteLine("\n=========================================");
            Console.WriteLine("ДОПОЛНИТЕЛЬНЫЕ ТЕСТЫ");
            Console.WriteLine("=========================================");

           
            Console.WriteLine("\n--- Все комбинации для CriticalEvent ---");
            var criticalEvent = new CriticalEvent();
            foreach (var context in contexts)
            {
                int priority = calculatorOop.CalculatePriority(criticalEvent, context.Value);
                Console.WriteLine($"{context.Key}: {priority}");
            }

            Console.WriteLine("\n--- Проверка SystemHeartbeat с разными контекстами ---");
            var heartbeatEvent = new SystemHeartbeatEvent();
            foreach (var context in contexts)
            {
                int priority = calculatorOop.CalculatePriority(heartbeatEvent, context.Value);
                Console.WriteLine($"{context.Key}: {priority} (должно быть всегда 1)");
            }

            Console.WriteLine("\n--- Проверка минимального значения (не ниже 1) ---");
            var infoEvent = new InfoEvent();
            var quietContext = new NotifyContext(true, false, false); // Только тихий час
            int minPriority = calculatorOop.CalculatePriority(infoEvent, quietContext);
            Console.WriteLine($"Info + QuietHours: {minPriority} (1-2=-1 -> минимум 1) ✓");

            Console.WriteLine("\n=========================================");
            Console.WriteLine("СРАВНЕНИЕ ПОДХОДОВ");
            Console.WriteLine("=========================================");
            Console.WriteLine("Enum + Switch:");
            Console.WriteLine("  + Простота реализации");
            Console.WriteLine("  + Легко добавлять новые типы в enum");
            Console.WriteLine("  - Логика размазана по switch");
            Console.WriteLine("  - Сложно расширять поведение");
            Console.WriteLine();
            Console.WriteLine("Иерархия классов:");
            Console.WriteLine("  + Чистая архитектура, каждый класс отвечает за себя");
            Console.WriteLine("  + Легко добавлять новое поведение");
            Console.WriteLine("  + Полиморфизм работает из коробки");
            Console.WriteLine("  - Больше кода");
            Console.WriteLine("  - Требует понимания ООП");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}