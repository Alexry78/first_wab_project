using UniversitySchedule.CLI.Models;  // ИЗМЕНИТЬ

namespace UniversitySchedule.CLI.Services;

public class ConflictDetector
{
    private readonly DatabaseService _db;

    public ConflictDetector(DatabaseService db)
    {
        _db = db;
    }

    public bool HasConflicts(Session newSession)
    {
        return _db.Sessions.Any(s => 
            s.Id != newSession.Id &&
            s.Date == newSession.Date &&
            (s.RoomId == newSession.RoomId ||
             s.TeacherId == newSession.TeacherId ||
             s.GroupId == newSession.GroupId) &&
            s.StartTime < newSession.EndTime &&
            s.EndTime > newSession.StartTime);
    }

    public List<string> GetConflictMessages(Session newSession)
    {
        var conflicts = new List<string>();
        
        foreach (var s in _db.Sessions.Where(s => s.Date == newSession.Date))
        {
            if (s.Id == newSession.Id) continue;
            
            if (s.RoomId == newSession.RoomId && TimesOverlap(s, newSession))
            {
                conflicts.Add($"Аудитория занята: {GetRoomName(s.RoomId)} {s.StartTime}-{s.EndTime}");
            }
            
            if (s.TeacherId == newSession.TeacherId && TimesOverlap(s, newSession))
            {
                conflicts.Add($"Преподаватель занят: {GetTeacherName(s.TeacherId)} {s.StartTime}-{s.EndTime}");
            }
            
            if (s.GroupId == newSession.GroupId && TimesOverlap(s, newSession))
            {
                conflicts.Add($"Группа занята: {GetGroupName(s.GroupId)} {s.StartTime}-{s.EndTime}");
            }
        }
        
        return conflicts;
    }

    private bool TimesOverlap(Session a, Session b) 
        => a.StartTime < b.EndTime && a.EndTime > b.StartTime;

    private string GetRoomName(int id) 
        => _db.Rooms.FirstOrDefault(r => r.Id == id)?.Code ?? "?";

    private string GetTeacherName(int id) 
        => _db.Teachers.FirstOrDefault(t => t.Id == id)?.Name ?? "?";

    private string GetGroupName(int id) 
        => _db.Groups.FirstOrDefault(g => g.Id == id)?.Code ?? "?";
}