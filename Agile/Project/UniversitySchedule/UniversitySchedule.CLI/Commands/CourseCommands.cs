using UniversitySchedule.CLI.Models;
using UniversitySchedule.CLI.Services;
using UniversitySchedule.CLI.Utils;

namespace UniversitySchedule.CLI.Commands;

public static class CourseCommands
{
    public static void Handle(string[] args, DatabaseService db)
    {
        if (args.Length < 2 || args[1] == "help")
        {
            ShowHelp();
            return;
        }

        var action = args[1].ToLower();

        switch (action)
        {
            case "add":
                AddCourse(args, db);
                break;
            case "list":
                ListCourses(db);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void AddCourse(string[] args, DatabaseService db)
    {
        string? title = null;
        string? code = null;
        int duration = 90;

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--title":
                    if (i + 1 < args.Length) title = args[++i];
                    break;
                case "--code":
                    if (i + 1 < args.Length) code = args[++i];
                    break;
                case "--duration":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int d)) duration = d;
                    break;
            }
        }

        if (string.IsNullOrEmpty(title))
        {
            ConsoleHelper.Error("Необходимо указать --title");
            return;
        }

        var course = new Course
        {
            Id = db.Courses.Count > 0 ? db.Courses.Max(c => c.Id) + 1 : 1,
            Title = title,
            Code = code,
            DurationMinutes = duration
        };

        db.Courses.Add(course);
        db.SaveAll();
        ConsoleHelper.Success($"Курс {title} (id={course.Id}) создан.");
    }

    private static void ListCourses(DatabaseService db)
    {
        if (!db.Courses.Any())
        {
            ConsoleHelper.Info("Нет добавленных курсов.");
            return;
        }

        Console.WriteLine("\nСписок курсов:");
        foreach (var c in db.Courses)
        {
            Console.WriteLine($"  [{c.Id}] {c.Title} - {c.DurationMinutes} мин. {(c.Code != null ? $"({c.Code})" : "")}");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("\nУправление курсами:");
        Console.WriteLine("  course add --title НАЗВАНИЕ [--code КОД] [--duration МИНУТЫ]");
        Console.WriteLine("  course list");
    }
}