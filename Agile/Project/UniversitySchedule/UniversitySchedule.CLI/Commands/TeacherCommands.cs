using UniversitySchedule.CLI.Models;
using UniversitySchedule.CLI.Services;
using UniversitySchedule.CLI.Utils;

namespace UniversitySchedule.CLI.Commands;

public static class TeacherCommands
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
                AddTeacher(args, db);
                break;
            case "list":
                ListTeachers(db);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void AddTeacher(string[] args, DatabaseService db)
    {
        string? name = null;
        string? email = null;
        string? department = null;

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--name":
                    if (i + 1 < args.Length) name = args[++i];
                    break;
                case "--email":
                    if (i + 1 < args.Length) email = args[++i];
                    break;
                case "--department":
                    if (i + 1 < args.Length) department = args[++i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(name))
        {
            ConsoleHelper.Error("Необходимо указать --name");
            return;
        }

        var teacher = new Teacher
        {
            Id = db.Teachers.Count > 0 ? db.Teachers.Max(t => t.Id) + 1 : 1,
            Name = name,
            Email = email,
            Department = department
        };

        db.Teachers.Add(teacher);
        db.SaveAll();
        ConsoleHelper.Success($"Преподаватель {name} (id={teacher.Id}) создан.");
    }

    private static void ListTeachers(DatabaseService db)
    {
        if (!db.Teachers.Any())
        {
            ConsoleHelper.Info("Нет добавленных преподавателей.");
            return;
        }

        Console.WriteLine("\nСписок преподавателей:");
        foreach (var t in db.Teachers)
        {
            Console.WriteLine($"  [{t.Id}] {t.Name} {(t.Email != null ? $"({t.Email})" : "")}");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("\nУправление преподавателями:");
        Console.WriteLine("  teacher add --name ФИО [--email EMAIL] [--department КАФЕДРА]");
        Console.WriteLine("  teacher list");
    }
}