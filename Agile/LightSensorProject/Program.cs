using System;

namespace LightSensorProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== КОНТРОЛЬ УРОВНЯ ОСВЕЩЁННОСТИ ===\n");
            Console.WriteLine("Демонстрация работы с событиями и делегатами\n");


            LightSensor sensor = new LightSensor();

            // Создаём подписчиков
            ConsoleDisplay display = new ConsoleDisplay();
            ComfortAdvisor advisor = new ComfortAdvisor();

            Console.WriteLine("\n--- Подписка на события ---");

            sensor.LightLevelChanged += display.OnLightLevelChanged;
            sensor.BlindingLightReached += display.OnBlindingLightReached;


            sensor.LightLevelChanged += advisor.OnLightLevelChanged;

            Console.WriteLine("\n--- Запуск сенсора ---\n");
            

            sensor.Start();

            Console.WriteLine("\n--- Отписка ConsoleDisplay от события BlindingLightReached ---");

            sensor.BlindingLightReached -= display.OnBlindingLightReached;

            Console.WriteLine("\n--- Повторный запуск сенсора (для демонстрации отписки) ---\n");
            
            
            sensor.Start();

            advisor.Report();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}