namespace UniversitySchedule.CLI.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int DurationMinutes { get; set; } = 90;
    
    public override string ToString() => $"{Title} ({DurationMinutes} мин)";
}