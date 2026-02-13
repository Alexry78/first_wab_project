using UniversitySchedule.CLI.Models;
using UniversitySchedule.CLI.Services;
using UniversitySchedule.CLI.Utils;

namespace UniversitySchedule.CLI.Commands;

public static class GroupCommands
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
                AddGroup(args, db);
                break;
            case "list":
                ListGroups(db);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void AddGroup(string[] args, DatabaseService db)
    {
        string? code = null;
        int size = 0;
        int year = DateTime.Now.Year;

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--code":
                    if (i + 1 < args.Length) code = args[++i];
                    break;
                case "--size":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int s)) size = s;
                    break;
                case "--year":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int y)) year = y;
                    break;
            }
        }

        if (string.IsNullOrEmpty(code) || size <= 0)
        {
            ConsoleHelper.Error("Необходимо указать --code и --size");
            return;
        }

        var group = new Group
        {
            Id = db.Groups.Count > 0 ? db.Groups.Max(g => g.Id) + 1 : 1,
            Code = code,
            Size = size,
            Year = year
        };

        db.Groups.Add(group);
        db.SaveAll();
        ConsoleHelper.Success($"Группа {code} (id={group.Id}) создана.");
    }

    private static void ListGroups(DatabaseService db)
    {
        if (!db.Groups.Any())
        {
            ConsoleHelper.Info("Нет добавленных групп.");
            return;
        }

        Console.WriteLine("\nСписок групп:");
        foreach (var g in db.Groups)
        {
            Console.WriteLine($"  [{g.Id}] {g.Code} - {g.Size} чел. ({g.Year} год)");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("\nУправление группами:");
        Console.WriteLine("  group add --code КОД --size КОЛИЧЕСТВО [--year ГОД]");
        Console.WriteLine("  group list");
    }
}