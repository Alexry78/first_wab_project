namespace UniversitySchedule.CLI.Models;

public class Group
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Size { get; set; }
    public int Year { get; set; }
    
    public override string ToString() => $"{Code} ({Year} год)";
}