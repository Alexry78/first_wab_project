using UniversitySchedule.CLI.Models;

public static class ConsoleHelper
{
    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    public static void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"! {message}");
        Console.ResetColor();
    }

    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ {message}");
        Console.ResetColor();
    }

    public static void PrintTable<T>(List<T> items, string[] headers, Func<T, string[]> rowSelector)
    {
        if (!items.Any())
        {
            Console.WriteLine("Нет данных для отображения.");
            return;
        }

        var rows = items.Select(rowSelector).ToList();
        var colWidths = headers.Select((h, i) => 
            rows.Max(r => r[i].Length) + 2).ToArray();

        // Верхняя граница
        Console.WriteLine("┌" + string.Join("┬", colWidths.Select(w => new string('─', w))) + "┐");
        
        // Заголовки
        Console.WriteLine("│" + string.Join("│", headers.Select((h, i) => $" {h.PadRight(colWidths[i] - 1)}")) + "│");
        
        // Разделитель
        Console.WriteLine("├" + string.Join("┼", colWidths.Select(w => new string('─', w))) + "┤");
        
        // Данные
        foreach (var row in rows)
        {
            Console.WriteLine("│" + string.Join("│", row.Select((cell, i) => $" {cell.PadRight(colWidths[i] - 1)}")) + "│");
        }
        
        // Нижняя граница
        Console.WriteLine("└" + string.Join("┴", colWidths.Select(w => new string('─', w))) + "┘");
    }
}