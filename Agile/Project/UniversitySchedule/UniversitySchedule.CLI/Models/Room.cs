namespace UniversitySchedule.CLI.Models;

public class Room
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Building { get; set; }
    
    public override string ToString() => $"{Code} ({Capacity} мест)";
}