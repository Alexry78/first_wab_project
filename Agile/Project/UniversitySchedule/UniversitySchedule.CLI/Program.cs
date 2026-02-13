using UniversitySchedule.CLI.Commands;
using UniversitySchedule.CLI.Services;
using UniversitySchedule.CLI.Utils;
using UniversitySchedule.CLI.Models;

namespace UniversitySchedule.CLI;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== УПРАВЛЕНИЕ РАСПИСАНИЕМ УНИВЕРСИТЕТА ===\n");
        
        var db = new DatabaseService();

        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "init":
                    InitDatabase(args.Skip(1).ToArray());
                    break;
                case "room":
                    RoomCommands.Handle(args, db);
                    break;
                case "teacher":
                    TeacherCommands.Handle(args, db);
                    break;
                case "group":
                    GroupCommands.Handle(args, db);
                    break;
                case "course":
                    CourseCommands.Handle(args, db);
                    break;
                case "session":
                    SessionCommands.Handle(args, db);
                    break;
                case "report":
                    ReportCommands.Handle(args, db);
                    break;
                case "help":
                    ShowHelp();
                    break;
                default:
                    ConsoleHelper.Error($"Неизвестная команда: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Ошибка: {ex.Message}");
        }
    }

    static void InitDatabase(string[] args)
    {
        string? dbPath = null;
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--db" && i + 1 < args.Length)
                dbPath = args[++i];
        }

        var path = dbPath ?? "data";
        Directory.CreateDirectory(path);
        
        var db = new DatabaseService(path);
        
        // Добавляем тестовые данные
        db.Rooms.Add(new Room { Id = 1, Code = "A-101", Capacity = 40, Building = "Главный" });
        db.Rooms.Add(new Room { Id = 2, Code = "B-202", Capacity = 60, Building = "Лабораторный" });
        
        db.Teachers.Add(new Teacher { Id = 1, Name = "Иванов И.И.", Email = "ivanov@university.ru", Department = "Информатика" });
        db.Teachers.Add(new Teacher { Id = 2, Name = "Петрова А.С.", Email = "petrova@university.ru", Department = "Математика" });
        
        db.Groups.Add(new Group { Id = 1, Code = "ИС-2025", Size = 30, Year = 2025 });
        db.Groups.Add(new Group { Id = 2, Code = "ПМ-2024", Size = 25, Year = 2024 });
        
        db.Courses.Add(new Course { Id = 1, Title = "Алгоритмы", Code = "CS201", DurationMinutes = 90 });
        db.Courses.Add(new Course { Id = 2, Title = "Математика", Code = "MA101", DurationMinutes = 90 });
        
        db.SaveAll();
        
        ConsoleHelper.Success($"База данных инициализирована в {path}");
        ConsoleHelper.Info("Добавлены тестовые данные: 2 аудитории, 2 преподавателя, 2 группы, 2 курса.");
    }

    static void ShowHelp()
    {
        Console.WriteLine("\nДоступные команды:");
        Console.WriteLine("  init                     - инициализация БД");
        Console.WriteLine("  room add/list            - управление аудиториями");
        Console.WriteLine("  teacher add/list         - управление преподавателями");
        Console.WriteLine("  group add/list           - управление группами");
        Console.WriteLine("  course add/list          - управление курсами");
        Console.WriteLine("  session add/list/delete  - управление занятиями");
        Console.WriteLine("  report group             - отчёты");
    }
}