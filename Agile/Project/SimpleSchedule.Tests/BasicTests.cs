using SimpleSchedule.Models;
using SimpleSchedule.Services;
using SimpleSchedule.Utils;

namespace SimpleSchedule.Tests;

public class BasicTests
{
    [Fact]
    public void RoomValidation_ValidData_ReturnsTrue()
    {
        var room = new Room { Code = "A-101", Capacity = 40 };
        var result = Validators.ValidateRoom(room, out _);
        Assert.True(result);
    }

    [Fact]
    public void RoomValidation_InvalidCapacity_ReturnsFalse()
    {
        var room = new Room { Code = "A-101", Capacity = -5 };
        var result = Validators.ValidateRoom(room, out string error);
        Assert.False(result);
        Assert.Contains("положительным", error);
    }

    [Fact]
    public void SessionValidation_StartAfterEnd_ReturnsFalse()
    {
        var session = new Session
        {
            StartTime = TimeSpan.FromHours(12),
            EndTime = TimeSpan.FromHours(10)
        };
        var result = Validators.ValidateSession(session, out string error);
        Assert.False(result);
        Assert.Contains("раньше", error);
    }

    [Fact]
    public void ConflictDetector_NoConflicts_ReturnsFalse()
    {
        var db = new DatabaseService(Path.GetTempPath());
        var detector = new ConflictDetector(db);
        
        var session1 = new Session { Id = 1, Date = DateTime.Today, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11) };
        var session2 = new Session { Id = 2, Date = DateTime.Today, StartTime = TimeSpan.FromHours(11), EndTime = TimeSpan.FromHours(12) };
        
        db.Sessions.Add(session1);
        
        var result = detector.HasConflicts(session2);
        Assert.False(result);
    }

    [Fact]
    public void ConflictDetector_RoomConflict_ReturnsTrue()
    {
        var db = new DatabaseService(Path.GetTempPath());
        var detector = new ConflictDetector(db);
        
        var session1 = new Session 
        { 
            Id = 1, 
            Date = DateTime.Today, 
            StartTime = TimeSpan.FromHours(10), 
            EndTime = TimeSpan.FromHours(12),
            RoomId = 101
        };
        
        var session2 = new Session 
        { 
            Id = 2, 
            Date = DateTime.Today, 
            StartTime = TimeSpan.FromHours(11), 
            EndTime = TimeSpan.FromHours(13),
            RoomId = 101
        };
        
        db.Sessions.Add(session1);
        
        var result = detector.HasConflicts(session2);
        Assert.True(result);
    }
}