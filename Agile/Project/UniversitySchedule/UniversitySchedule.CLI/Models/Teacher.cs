namespace UniversitySchedule.CLI.Models;

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Department { get; set; }
    
    public override string ToString() => Name;
}