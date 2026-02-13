using UniversitySchedule.CLI.Models;  // ИЗМЕНИТЬ
using System.Text.Json;

namespace UniversitySchedule.CLI.Services;

public class DatabaseService
{
    private readonly string _dataPath;
    
    public List<Room> Rooms { get; private set; } = new();
    public List<Teacher> Teachers { get; private set; } = new();
    public List<Group> Groups { get; private set; } = new();
    public List<Course> Courses { get; private set; } = new();
    public List<Session> Sessions { get; private set; } = new();

    public DatabaseService(string dataPath = "data")
    {
        _dataPath = dataPath;
        Directory.CreateDirectory(_dataPath);
        LoadAll();
    }

    public void SaveAll()
    {
        SaveToFile("rooms.json", Rooms);
        SaveToFile("teachers.json", Teachers);
        SaveToFile("groups.json", Groups);
        SaveToFile("courses.json", Courses);
        SaveToFile("sessions.json", Sessions);
    }

    private void LoadAll()
    {
        Rooms = LoadFromFile<List<Room>>("rooms.json") ?? new();
        Teachers = LoadFromFile<List<Teacher>>("teachers.json") ?? new();
        Groups = LoadFromFile<List<Group>>("groups.json") ?? new();
        Courses = LoadFromFile<List<Course>>("courses.json") ?? new();
        Sessions = LoadFromFile<List<Session>>("sessions.json") ?? new();
    }

    private void SaveToFile<T>(string filename, T data)
    {
        var path = Path.Combine(_dataPath, filename);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private T? LoadFromFile<T>(string filename)
    {
        var path = Path.Combine(_dataPath, filename);
        if (!File.Exists(path)) return default;
        
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json);
    }

    public int GetNextId<T>()
    {
        return typeof(T).Name switch
        {
            nameof(Room) => Rooms.Count > 0 ? Rooms.Max(r => r.Id) + 1 : 1,
            nameof(Teacher) => Teachers.Count > 0 ? Teachers.Max(t => t.Id) + 1 : 1,
            nameof(Group) => Groups.Count > 0 ? Groups.Max(g => g.Id) + 1 : 1,
            nameof(Course) => Courses.Count > 0 ? Courses.Max(c => c.Id) + 1 : 1,
            nameof(Session) => Sessions.Count > 0 ? Sessions.Max(s => s.Id) + 1 : 1,
            _ => 1
        };
    }
}