using UniversitySchedule.CLI.Models;      // ИЗМЕНИТЬ
using UniversitySchedule.CLI.Services;    // ИЗМЕНИТЬ
using UniversitySchedule.CLI.Utils;       // ИЗМЕНИТЬ

namespace UniversitySchedule.CLI.Commands;

public static class SessionCommands
{
    public static void Handle(string[] args, DatabaseService db)
    {
        if (args.Length < 2)
        {
            ShowHelp();
            return;
        }

        var action = args[1].ToLower();

        switch (action)
        {
            case "add":
                AddSession(args.Skip(2).ToArray(), db);
                break;
            case "list":
                ListSessions(args.Skip(2).ToArray(), db);
                break;
            case "delete":
                DeleteSession(args.Skip(2).ToArray(), db);
                break;
            case "find-conflicts":
                FindConflicts(db);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void AddSession(string[] args, DatabaseService db)
    {
        int? courseId = null;
        int? teacherId = null;
        int? groupId = null;
        int? roomId = null;
        DateTime? date = null;
        TimeSpan? start = null;
        TimeSpan? end = null;
        string? notes = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--course":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int c)) courseId = c;
                    break;
                case "--teacher":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int t)) teacherId = t;
                    break;
                case "--group":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int g)) groupId = g;
                    break;
                case "--room":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int r)) roomId = r;
                    break;
                case "--date":
                    if (i + 1 < args.Length && DateTime.TryParse(args[++i], out DateTime d)) date = d;
                    break;
                case "--start":
                    if (i + 1 < args.Length && TimeSpan.TryParse(args[++i], out TimeSpan st)) start = st;
                    break;
                case "--end":
                    if (i + 1 < args.Length && TimeSpan.TryParse(args[++i], out TimeSpan et)) end = et;
                    break;
                case "--notes":
                    if (i + 1 < args.Length) notes = args[++i];
                    break;
            }
        }

        // Проверка обязательных параметров
        if (!courseId.HasValue || !teacherId.HasValue || !groupId.HasValue || 
            !roomId.HasValue || !date.HasValue || !start.HasValue || !end.HasValue)
        {
            ConsoleHelper.Error("Необходимо указать все параметры: --course, --teacher, --group, --room, --date, --start, --end");
            return;
        }

        // Проверка существования связанных сущностей
        if (!db.Courses.Any(c => c.Id == courseId.Value))
        {
            ConsoleHelper.Error($"Курс с ID {courseId} не найден");
            return;
        }

        if (!db.Teachers.Any(t => t.Id == teacherId.Value))
        {
            ConsoleHelper.Error($"Преподаватель с ID {teacherId} не найден");
            return;
        }

        if (!db.Groups.Any(g => g.Id == groupId.Value))
        {
            ConsoleHelper.Error($"Группа с ID {groupId} не найдена");
            return;
        }

        if (!db.Rooms.Any(r => r.Id == roomId.Value))
        {
            ConsoleHelper.Error($"Аудитория с ID {roomId} не найдена");
            return;
        }

        var session = new Session
        {
            Id = db.GetNextId<Session>(),
            CourseId = courseId.Value,
            TeacherId = teacherId.Value,
            GroupId = groupId.Value,
            RoomId = roomId.Value,
            Date = date.Value,
            StartTime = start.Value,
            EndTime = end.Value,
            Notes = notes
        };

        // Валидация времени
        if (!Validators.ValidateSession(session, out string validationError))
        {
            ConsoleHelper.Error(validationError);
            return;
        }

        // Проверка конфликтов
        var detector = new ConflictDetector(db);
        if (detector.HasConflicts(session))
        {
            ConsoleHelper.Error("Обнаружены конфликты:");
            foreach (var msg in detector.GetConflictMessages(session))
            {
                Console.WriteLine($"  • {msg}");
            }
            ConsoleHelper.Info("Используйте другие параметры для создания занятия.");
            return;
        }

        db.Sessions.Add(session);
        db.SaveAll();
        
        var course = db.Courses.First(c => c.Id == courseId.Value);
        var teacher = db.Teachers.First(t => t.Id == teacherId.Value);
        var group = db.Groups.First(g => g.Id == groupId.Value);
        var room = db.Rooms.First(r => r.Id == roomId.Value);
        
        ConsoleHelper.Success($"Занятие создано: id={session.Id}, {session.Date:yyyy-MM-dd} {session.StartTime}-{session.EndTime}, {room.Code}, {teacher.Name}, {group.Code}");
    }

    private static void ListSessions(string[] args, DatabaseService db)
    {
        var sessions = db.Sessions.AsEnumerable();

        // Фильтры
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--group":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int groupId))
                        sessions = sessions.Where(s => s.GroupId == groupId);
                    break;
                case "--teacher":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int teacherId))
                        sessions = sessions.Where(s => s.TeacherId == teacherId);
                    break;
                case "--room":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int roomId))
                        sessions = sessions.Where(s => s.RoomId == roomId);
                    break;
                case "--date":
                    if (i + 1 < args.Length && DateTime.TryParse(args[++i], out DateTime date))
                        sessions = sessions.Where(s => s.Date == date);
                    break;
            }
        }

        var sessionList = sessions.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList();

        if (!sessionList.Any())
        {
            ConsoleHelper.Info("Нет занятий по заданным критериям.");
            return;
        }

        var headers = new[] { "ID", "Дата", "Время", "Предмет", "Преподаватель", "Группа", "Аудитория" };
        
        ConsoleHelper.PrintTable(sessionList, headers, s =>
        {
            var course = db.Courses.FirstOrDefault(c => c.Id == s.CourseId);
            var teacher = db.Teachers.FirstOrDefault(t => t.Id == s.TeacherId);
            var group = db.Groups.FirstOrDefault(g => g.Id == s.GroupId);
            var room = db.Rooms.FirstOrDefault(r => r.Id == s.RoomId);

            return new[]
            {
                s.Id.ToString(),
                s.Date.ToString("yyyy-MM-dd"),
                $"{s.StartTime}-{s.EndTime}",
                course?.Title ?? "?",
                teacher?.Name ?? "?",
                group?.Code ?? "?",
                room?.Code ?? "?"
            };
        });
    }

    private static void DeleteSession(string[] args, DatabaseService db)
    {
        if (args.Length == 0)
        {
            ConsoleHelper.Error("Укажите ID занятия");
            return;
        }

        if (!int.TryParse(args[0], out int id))
        {
            ConsoleHelper.Error("Некорректный ID");
            return;
        }

        var session = db.Sessions.FirstOrDefault(s => s.Id == id);
        if (session == null)
        {
            ConsoleHelper.Error($"Занятие с ID {id} не найдено.");
            return;
        }

        db.Sessions.Remove(session);
        db.SaveAll();
        ConsoleHelper.Success($"Занятие {id} удалено.");
    }

    private static void FindConflicts(DatabaseService db)
    {
        var detector = new ConflictDetector(db);
        var allConflicts = new List<string>();

        foreach (var session in db.Sessions)
        {
            var conflicts = detector.GetConflictMessages(session);
            allConflicts.AddRange(conflicts);
        }

        allConflicts = allConflicts.Distinct().ToList();

        if (!allConflicts.Any())
        {
            ConsoleHelper.Success("Конфликтов не обнаружено!");
            return;
        }

        ConsoleHelper.Warning($"Найдено {allConflicts.Count} конфликтов:");
        foreach (var conflict in allConflicts)
        {
            Console.WriteLine($"  • {conflict}");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Управление занятиями:");
        Console.WriteLine("  session add --course ID --teacher ID --group ID --room ID --date YYYY-MM-DD --start HH:MM --end HH:MM [--notes ТЕКСТ]");
        Console.WriteLine("  session list [--group ID] [--teacher ID] [--room ID] [--date DATE]");
        Console.WriteLine("  session delete ID");
        Console.WriteLine("  session find-conflicts");
    }
}