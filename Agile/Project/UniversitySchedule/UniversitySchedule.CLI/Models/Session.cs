namespace UniversitySchedule.CLI.Models;

public class Session
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int TeacherId { get; set; }
    public int GroupId { get; set; }
    public int RoomId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
    
    public DateTime StartDateTime => Date.Add(StartTime);
    public DateTime EndDateTime => Date.Add(EndTime);
    
    public override string ToString() 
        => $"{Date:yyyy-MM-dd} {StartTime:hh\\:mm}-{EndTime:hh\\:mm}";
}