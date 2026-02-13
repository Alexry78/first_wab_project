using UniversitySchedule.CLI.Models;
using UniversitySchedule.CLI.Services;
using UniversitySchedule.CLI.Utils;

namespace UniversitySchedule.CLI.Commands;

public static class ReportCommands
{
    public static void Handle(string[] args, DatabaseService db)
    {
        if (args.Length < 2 || args[1] == "help")
        {
            ShowHelp();
            return;
        }

        var reportType = args[1].ToLower();

        switch (reportType)
        {
            case "group":
                GroupReport(args, db);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void GroupReport(string[] args, DatabaseService db)
    {
        int? groupId = null;
        DateTime? from = null;
        DateTime? to = null;
        string? format = "text";

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--group":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int g)) groupId = g;
                    break;
                case "--from":
                    if (i + 1 < args.Length && DateTime.TryParse(args[++i], out DateTime f)) from = f;
                    break;
                case "--to":
                    if (i + 1 < args.Length && DateTime.TryParse(args[++i], out DateTime t)) to = t;
                    break;
                case "--format":
                    if (i + 1 < args.Length) format = args[++i];
                    break;
            }
        }

        if (!groupId.HasValue || !from.HasValue || !to.HasValue)
        {
            ConsoleHelper.Error("Необходимо указать --group, --from, --to");
            return;
        }

        var exporter = new ScheduleExporter(db);  // Здесь используется ScheduleExporter

        switch (format?.ToLower())
        {
            case "text":
                Console.Write(exporter.ExportGroupSchedule(groupId.Value, from.Value, to.Value));
                break;
            case "csv":
                Console.Write(exporter.ExportToCsv(groupId.Value, from.Value, to.Value));
                break;
            default:
                ConsoleHelper.Error($"Неизвестный формат: {format}");
                break;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("\nОтчёты:");
        Console.WriteLine("  report group --group ID --from YYYY-MM-DD --to YYYY-MM-DD [--format text|csv]");
    }
}