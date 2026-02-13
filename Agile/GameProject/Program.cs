using System;
using System.Collections.Generic;

namespace GameProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация игровых сущностей ===\n");

            // Создание различных сущностей
            Player hero = new Player("Артур", 100);
            Monster goblin = new Monster("Гоблин-разбойник", 50, "Гоблин", 15);
            Monster dragon = new Monster("Огненный дракон", 200, "Дракон", 40);
            NPC merchant = new NPC("Торговец Боб", 30, "Торговец", "Лучшие товары по лучшим ценам!");
            NPC guard = new NPC("Стражник", 80, "Стражник", "Проходи, путник, и не шали.");

            // Демонстрация полиморфизма - список всех сущностей
            List<GameEntity> entities = new List<GameEntity> { hero, goblin, dragon, merchant, guard };

            Console.WriteLine("=== Все сущности в игре ===");
            foreach (var entity in entities)
            {
                Console.WriteLine(entity);
            }

            Console.WriteLine("\n=== Действия сущностей (полиморфизм) ===");
            foreach (var entity in entities)
            {
                Console.WriteLine(entity.Act());
            }

            Console.WriteLine("\n=== Сражение героя с гоблином ===");
            Console.WriteLine($"Начальное состояние: {hero}");
            Console.WriteLine($"Начальное состояние: {goblin}");

            // Бой до победы
            while (hero.IsAlive() && goblin.IsAlive())
            {
                hero.Attack(goblin);
                if (goblin.IsAlive())
                {
                    goblin.Attack(hero);
                }
                Console.WriteLine($"Состояние: Герой {hero.Health}/{goblin.Name} {goblin.Health}");
                Console.WriteLine("---");
            }

            Console.WriteLine("\n=== Итог сражения ===");
            Console.WriteLine(hero);
            Console.WriteLine(goblin);

            Console.WriteLine("\n=== Разговор с NPC ===");
            merchant.Talk();
            guard.Talk();

            Console.WriteLine("\n=== Демонстрация метода TakeDamage с отрицательным уроном ===");
            Console.WriteLine($"{hero.Name} до лечения: {hero.Health}");
            hero.TakeDamage(-30); // Лечение
            Console.WriteLine($"{hero.Name} после лечения: {hero.Health}");

            Console.WriteLine("\n=== Демонстрация проверки жизни ===");
            Console.WriteLine($"{hero.Name} жив? {hero.IsAlive()}");
            
            Monster weakMonster = new Monster("Слабый слизень", 5, "Слизень", 1);
            Console.WriteLine($"{weakMonster}");
            weakMonster.TakeDamage(10);
            Console.WriteLine($"После смертельного урона: {weakMonster}");
            Console.WriteLine($"{weakMonster.Name} жив? {weakMonster.IsAlive()}");
            Console.WriteLine($"Действие мертвого монстра: {weakMonster.Act()}");

            Console.WriteLine("\n=== Валидация данных ===");
            try
            {
                Console.WriteLine("Попытка создать сущность с пустым именем:");
                GameEntity badEntity = new GameEntity("", 100);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            try
            {
                Console.WriteLine("\nПопытка создать сущность с отрицательным здоровьем:");
                GameEntity badEntity = new GameEntity("Тест", -10);
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