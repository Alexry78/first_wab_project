using UniversitySchedule.CLI.Models;  // ИЗМЕНИТЬ

namespace UniversitySchedule.CLI.Utils;

public static class Validators
{
    public static bool ValidateSession(Session session, out string error)
    {
        if (session.StartTime >= session.EndTime)
        {
            error = "Время начала должно быть раньше времени окончания";
            return false;
        }

        if (session.StartTime < TimeSpan.FromHours(8) || session.EndTime > TimeSpan.FromHours(22))
        {
            error = "Занятия могут проходить только с 8:00 до 22:00";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool ValidateRoom(Room room, out string error)
    {
        if (string.IsNullOrWhiteSpace(room.Code))
        {
            error = "Код аудитории не может быть пустым";
            return false;
        }

        if (room.Capacity <= 0)
        {
            error = "Вместимость должна быть положительным числом";
            return false;
        }

        error = string.Empty;
        return true;
    }
}