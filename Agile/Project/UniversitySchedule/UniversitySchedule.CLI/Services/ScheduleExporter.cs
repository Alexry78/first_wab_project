using UniversitySchedule.CLI.Models;
using System.Text;

namespace UniversitySchedule.CLI.Services;

public class ScheduleExporter
{
    private readonly DatabaseService _db;

    public ScheduleExporter(DatabaseService db)
    {
        _db = db;
    }

    public string ExportGroupSchedule(int groupId, DateTime from, DateTime to)
    {
        var sessions = _db.Sessions
            .Where(s => s.GroupId == groupId && s.Date >= from && s.Date <= to)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToList();

        var group = _db.Groups.FirstOrDefault(g => g.Id == groupId);
        
        var sb = new StringBuilder();
        sb.AppendLine($"Расписание группы {group?.Code ?? "?"}");
        sb.AppendLine($"Период: {from:yyyy-MM-dd} - {to:yyyy-MM-dd}");
        sb.AppendLine();

        if (!sessions.Any())
        {
            sb.AppendLine("Нет занятий за указанный период.");
            return sb.ToString();
        }

        foreach (var s in sessions)
        {
            var course = _db.Courses.FirstOrDefault(c => c.Id == s.CourseId);
            var teacher = _db.Teachers.FirstOrDefault(t => t.Id == s.TeacherId);
            var room = _db.Rooms.FirstOrDefault(r => r.Id == s.RoomId);
            
            sb.AppendLine($"{s.Date:yyyy-MM-dd} {s.StartTime}-{s.EndTime}: {course?.Title} ({teacher?.Name}, {room?.Code})");
        }

        return sb.ToString();
    }

    public string ExportToCsv(int groupId, DateTime from, DateTime to)
    {
        var sessions = _db.Sessions
            .Where(s => s.GroupId == groupId && s.Date >= from && s.Date <= to)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("Date,Start,End,Course,Teacher,Room");
        
        foreach (var s in sessions)
        {
            var course = _db.Courses.FirstOrDefault(c => c.Id == s.CourseId);
            var teacher = _db.Teachers.FirstOrDefault(t => t.Id == s.TeacherId);
            var room = _db.Rooms.FirstOrDefault(r => r.Id == s.RoomId);
            
            sb.AppendLine($"{s.Date:yyyy-MM-dd},{s.StartTime},{s.EndTime},{course?.Title ?? "?"},{teacher?.Name ?? "?"},{room?.Code ?? "?"}");
        }
        
        return sb.ToString();
    }
}