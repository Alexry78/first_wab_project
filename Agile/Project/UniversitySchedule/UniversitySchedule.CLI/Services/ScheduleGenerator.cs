using UniversitySchedule.CLI.Models;
using System.Text;

namespace UniversitySchedule.CLI.Services;

public class ScheduleGenerator
{
    private readonly DatabaseService _db;

    public ScheduleGenerator(DatabaseService db)
    {
        _db = db;
    }

    // Здесь добавьте методы генерации расписания
    // Например:

    public string GenerateGroupSchedule(int groupId, DateTime from, DateTime to)
    {
        var sessions = _db.Sessions
            .Where(s => s.GroupId == groupId && s.Date >= from && s.Date <= to)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToList();

        if (!sessions.Any())
        {
            return "Нет занятий за указанный период.";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Расписание для группы {_db.Groups.FirstOrDefault(g => g.Id == groupId)?.Code}:");
        
        foreach (var s in sessions)
        {
            var course = _db.Courses.FirstOrDefault(c => c.Id == s.CourseId);
            var teacher = _db.Teachers.FirstOrDefault(t => t.Id == s.TeacherId);
            var room = _db.Rooms.FirstOrDefault(r => r.Id == s.RoomId);
            
            sb.AppendLine($"  {s.Date:yyyy-MM-dd} {s.StartTime}-{s.EndTime}: {course?.Title} ({teacher?.Name}, {room?.Code})");
        }

        return sb.ToString();
    }
}