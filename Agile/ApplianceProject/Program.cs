using System;
using System.Collections.Generic;

namespace ApplianceProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация работы классов бытовой техники ===\n");

            // Создаем холодильники
            Refrigerator fridge1 = new Refrigerator("Samsung", 150, 4);
            Refrigerator fridge2 = new Refrigerator("LG", 180, -18);
            Refrigerator fridge3 = new Refrigerator("Bosch", 200, 2);

            // Создаем стиральные машины
            WashingMachine washer1 = new WashingMachine("Indesit", 2200, "Хлопок 60°C");
            WashingMachine washer2 = new WashingMachine("Samsung", 2000, "Деликатная 30°C");
            WashingMachine washer3 = new WashingMachine("LG", 2100, "Быстрая 15 мин");

            // Создаем список всей техники для полиморфного вызова
            List<Appliance> appliances = new List<Appliance> { fridge1, fridge2, fridge3, washer1, washer2, washer3 };

            Console.WriteLine("=== Вся техника (общая информация) ===");
            foreach (var appliance in appliances)
            {
                Console.WriteLine(appliance);
            }

            Console.WriteLine("\n=== Информация об использовании холодильников ===");
            Console.WriteLine(fridge1.UsageInfo());
            Console.WriteLine(fridge2.UsageInfo());
            Console.WriteLine(fridge3.UsageInfo());

            Console.WriteLine("\n=== Информация об использовании стиральных машин ===");
            Console.WriteLine(washer1.UsageInfo());
            Console.WriteLine(washer2.UsageInfo());
            Console.WriteLine(washer3.UsageInfo());

            Console.WriteLine("\n=== Демонстрация полиморфизма ===");
            Console.WriteLine("Вызов UsageInfo() через базовый класс:");
            foreach (var appliance in appliances)
            {
                Console.WriteLine(appliance.UsageInfo());
            }

            Console.WriteLine("\n=== Изменение параметров техники ===");
            
            // Меняем температуру холодильника
            Console.WriteLine($"\nБыло: {fridge1}");
            fridge1.TemperatureMode = -5;
            fridge1.ConsumptionWatts = 160;
            Console.WriteLine($"Стало: {fridge1}");
            Console.WriteLine($"Обновленная информация: {fridge1.UsageInfo()}");

            // Меняем цикл стирки стиральной машины
            Console.WriteLine($"\nБыло: {washer2}");
            washer2.WashCycle = "Шерсть 40°C";
            washer2.ConsumptionWatts = 1950;
            Console.WriteLine($"Стало: {washer2}");
            Console.WriteLine($"Обновленная информация: {washer2.UsageInfo()}");

            Console.WriteLine("\n=== Демонстрация работы с коллекцией ===");
            Console.WriteLine("Холодильники с температурой ниже 0°C:");
            foreach (var appliance in appliances)
            {
                if (appliance is Refrigerator fridge && fridge.TemperatureMode < 0)
                {
                    Console.WriteLine($"- {fridge.ApplianceMake}: {fridge.TemperatureMode}°C");
                }
            }

            Console.WriteLine("\nСтиральные машины с мощностью более 2000 Вт:");
            foreach (var appliance in appliances)
            {
                if (appliance is WashingMachine washer && washer.ConsumptionWatts > 2000)
                {
                    Console.WriteLine($"- {washer.ApplianceMake}: {washer.ConsumptionWatts} Вт, цикл: {washer.WashCycle}");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}