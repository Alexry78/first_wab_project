using System;

namespace CircleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация работы класса Circle ===\n");

            // Создание круга с радиусом 5
            Circle circle1 = new Circle(5);
            Console.WriteLine("Круг 1:");
            Console.WriteLine($"Радиус: {circle1.Radius}");
            Console.WriteLine($"Площадь: {circle1.CalculateArea():F2}");
            Console.WriteLine($"Длина окружности: {circle1.CalculateCircumference():F2}");
            Console.WriteLine($"Строковое представление: {circle1}");
            Console.WriteLine();

            // Создание круга с радиусом 7.5
            Circle circle2 = new Circle(7.5);
            Console.WriteLine("Круг 2:");
            Console.WriteLine($"Радиус: {circle2.Radius}");
            Console.WriteLine($"Площадь: {circle2.CalculateArea():F2}");
            Console.WriteLine($"Длина окружности: {circle2.CalculateCircumference():F2}");
            Console.WriteLine($"Строковое представление: {circle2}");
            Console.WriteLine();

            // Демонстрация изменения радиуса
            Console.WriteLine("Изменяем радиус первого круга на 10...");
            circle1.Radius = 10;
            Console.WriteLine($"Новое строковое представление: {circle1}");
            Console.WriteLine();

            // Демонстрация обработки некорректного радиуса
            Console.WriteLine("Попытка установить отрицательный радиус:");
            try
            {
                circle1.Radius = -3;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}