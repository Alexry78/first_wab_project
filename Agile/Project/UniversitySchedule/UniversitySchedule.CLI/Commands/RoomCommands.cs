using UniversitySchedule.CLI.Models;
using UniversitySchedule.CLI.Services;
using UniversitySchedule.CLI.Utils;

namespace UniversitySchedule.CLI.Commands;

public static class RoomCommands
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
                AddRoom(args, db);
                break;
            case "list":
                ListRooms(db);
                break;
            case "show":
                ShowRoom(args, db);
                break;
            case "delete":
                DeleteRoom(args, db);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void AddRoom(string[] args, DatabaseService db)
    {
        string? code = null;
        int capacity = 0;
        string? building = null;

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--code":
                    if (i + 1 < args.Length) code = args[++i];
                    break;
                case "--capacity":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int cap)) capacity = cap;
                    break;
                case "--building":
                    if (i + 1 < args.Length) building = args[++i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(code) || capacity <= 0)
        {
            ConsoleHelper.Error("Необходимо указать --code и --capacity");
            return;
        }

        var room = new Room
        {
            Id = db.Rooms.Count > 0 ? db.Rooms.Max(r => r.Id) + 1 : 1,
            Code = code,
            Capacity = capacity,
            Building = building
        };

        db.Rooms.Add(room);
        db.SaveAll();
        ConsoleHelper.Success($"Аудитория {code} (id={room.Id}) создана.");
    }

    private static void ListRooms(DatabaseService db)
    {
        if (!db.Rooms.Any())
        {
            ConsoleHelper.Info("Нет добавленных аудиторий.");
            return;
        }

        Console.WriteLine("\nСписок аудиторий:");
        foreach (var room in db.Rooms)
        {
            Console.WriteLine($"  [{room.Id}] {room.Code} - {room.Capacity} мест {(room.Building != null ? $"({room.Building})" : "")}");
        }
    }

    private static void ShowRoom(string[] args, DatabaseService db)
    {
        if (args.Length < 3)
        {
            ConsoleHelper.Error("Укажите ID или код аудитории");
            return;
        }

        var identifier = args[2];
        var room = db.Rooms.FirstOrDefault(r => 
            r.Id.ToString() == identifier || r.Code == identifier);

        if (room == null)
        {
            ConsoleHelper.Error($"Аудитория '{identifier}' не найдена.");
            return;
        }

        Console.WriteLine($"ID: {room.Id}");
        Console.WriteLine($"Код: {room.Code}");
        Console.WriteLine($"Вместимость: {room.Capacity}");
        Console.WriteLine($"Здание: {room.Building ?? "-"}");
        
        var sessions = db.Sessions.Count(s => s.RoomId == room.Id);
        Console.WriteLine($"Используется в занятиях: {sessions}");
    }

    private static void DeleteRoom(string[] args, DatabaseService db)
    {
        if (args.Length < 3)
        {
            ConsoleHelper.Error("Укажите ID или код аудитории");
            return;
        }

        var identifier = args[2];
        var room = db.Rooms.FirstOrDefault(r => 
            r.Id.ToString() == identifier || r.Code == identifier);

        if (room == null)
        {
            ConsoleHelper.Error($"Аудитория '{identifier}' не найдена.");
            return;
        }

        if (db.Sessions.Any(s => s.RoomId == room.Id))
        {
            ConsoleHelper.Error("Нельзя удалить аудиторию, которая используется в занятиях.");
            return;
        }

        db.Rooms.Remove(room);
        db.SaveAll();
        ConsoleHelper.Success($"Аудитория {identifier} удалена.");
    }

    private static void ShowHelp()
    {
        Console.WriteLine("\nУправление аудиториями:");
        Console.WriteLine("  room add --code КОД --capacity ВМЕСТИМОСТЬ [--building ЗДАНИЕ]");
        Console.WriteLine("  room list");
        Console.WriteLine("  room show <ID|КОД>");
        Console.WriteLine("  room delete <ID|КОД>");
    }
}