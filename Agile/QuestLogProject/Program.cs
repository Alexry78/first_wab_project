using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestLogProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ЖУРНАЛ КВЕСТОВ RPG ===\n");


            QuestLog questLog = new QuestLog();


            var objectives1 = new List<Objective>
            {
                new Objective("talk_chief", "Поговорить с вождём племени"),
                new Objective("collect_totems", "Собрать 5 тотемов", 5),
                new Objective("defeat_spirit", "Победить духа предков")
            };

            var objectives2 = new List<Objective>
            {
                new Objective("kill_wolves", "Убить 10 волков", 10),
                new Objective("find_den", "Найти логово вожака"),
                new Objective("collect_pelts", "Собрать 5 шкур", 5)
            };

            var objectives3 = new List<Objective>
            {
                new Objective("gather_herbs", "Собрать 15 лечебных трав", 15),
                new Objective("brew_potions", "Сварить 3 зелья", 3),
                new Objective("heal_villagers", "Вылечить 5 больных крестьян", 5)
            };

            var objectives4 = new List<Objective>
            {
                new Objective("clear_dungeon", "Очистить подземелье от нежити"),
                new Objective("find_artifact", "Найти древний артефакт"),
                new Objective("escape_cursed", "Покинуть проклятое подземелье живым")
            };


            Quest quest1 = new Quest("q001", "Духи предков", Difficulty.Hard, objectives1);
            Quest quest2 = new Quest("q002", "Волчья стая", Difficulty.Normal, objectives2);
            Quest quest3 = new Quest("q003", "Целитель деревни", Difficulty.Easy, objectives3);
            Quest quest4 = new Quest("q004", "Проклятое подземелье", Difficulty.Nightmare, objectives4);


            Console.WriteLine("--- Добавление квестов ---");
            questLog.Add(quest1);
            questLog.Add(quest2);
            questLog.Add(quest3);
            questLog.Add(quest4);
            Console.WriteLine($"Всего квестов: {questLog.Count}\n");

            Console.WriteLine("--- Доступ по индексу ---");
            try
            {
                for (int i = 0; i < questLog.Count + 1; i++) 
                {
                    Console.WriteLine($"[{i}]: {questLog[i]}");
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Console.WriteLine();

       
            Console.WriteLine("--- Доступ по ID ---");
            try
            {
                Console.WriteLine($"ID 'q003': {questLog["q003"]}");
                Console.WriteLine($"ID 'q001': {questLog["q001"]}");
                Console.WriteLine($"ID 'q999': {questLog["q999"]}"); 
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Console.WriteLine();


            Console.WriteLine("--- Все квесты (foreach) ---");
            foreach (var quest in questLog)
            {
                Console.WriteLine(quest);
            }
            Console.WriteLine();


            Console.WriteLine("--- Квесты сложности Hard и выше (yield return) ---");
            foreach (var quest in questLog.EnumerateByDifficulty(Difficulty.Hard))
            {
                Console.WriteLine($"  {quest.Title} (Сложность: {quest.Difficulty})");
                
                Console.WriteLine("    Цели:");
                foreach (var objective in quest.Objectives)
                {
                    Console.WriteLine($"      - {objective}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("--- Удаление квеста по индексу 1 ---");
            bool removed = questLog.RemoveAt(1);
            Console.WriteLine($"Удаление {(removed ? "успешно" : "не удалось")}");
            Console.WriteLine($"Осталось квестов: {questLog.Count}");
            Console.WriteLine("Оставшиеся квесты:");
            foreach (var quest in questLog)
            {
                Console.WriteLine($"  {quest}");
            }
            Console.WriteLine();

            Console.WriteLine("--- Удаление квеста по ID 'q003' ---");
            removed = questLog.RemoveById("q003");
            Console.WriteLine($"Удаление {(removed ? "успешно" : "не удалось")}");
            Console.WriteLine($"Осталось квестов: {questLog.Count}");
            Console.WriteLine("Оставшиеся квесты:");
            foreach (var quest in questLog)
            {
                Console.WriteLine($"  {quest}");
            }
            Console.WriteLine();

            Console.WriteLine("--- Попытка добавить квест с существующим ID 'q001' ---");
            try
            {
                Quest duplicateQuest = new Quest("q001", "Дубликат", Difficulty.Trivial);
                questLog.Add(duplicateQuest);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Console.WriteLine();

            Console.WriteLine("--- Попытка изменить цели квеста (должна быть ошибка компиляции) ---");
            Console.WriteLine("// quest1.Objectives.Add(new Objective(\"test\", \"Test\")); // Ошибка компиляции!");
            Console.WriteLine("Цели квеста доступны только для чтения:");
            foreach (var obj in quest1.Objectives)
            {
                Console.WriteLine($"  {obj}");
            }
            Console.WriteLine();
           
            Console.WriteLine("--- Попытка удалить несуществующий квест по ID 'q999' ---");
            removed = questLog.RemoveById("q999");
            Console.WriteLine($"Удаление {(removed ? "успешно" : "не удалось")}");
            Console.WriteLine();

            Console.WriteLine("--- Поиск квеста через TryGetQuest ---");
            if (questLog.TryGetQuest("q001", out Quest? foundQuest))
            {
                Console.WriteLine($"Найден квест: {foundQuest}");
            }
            else
            {
                Console.WriteLine("Квест не найден");
            }

            if (questLog.TryGetQuest("q999", out foundQuest))
            {
                Console.WriteLine($"Найден квест: {foundQuest}");
            }
            else
            {
                Console.WriteLine("Квест 'q999' не найден");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}