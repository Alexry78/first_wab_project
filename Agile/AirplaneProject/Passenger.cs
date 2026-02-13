using System;

namespace AirplaneProject
{
    /// <summary>
    /// Класс, представляющий пассажира
    /// </summary>
    public class Passenger
    {
        // Атрибуты пассажира
        private string _fullName;        // ФИО пассажира
        private string _ticketNumber;    // Номер полётного билета
        private Seat _assignedSeat;       // Место посадки

        /// <summary>
        /// Конструктор пассажира
        /// </summary>
        /// <param name="fullName">ФИО пассажира</param>
        /// <param name="ticketNumber">Номер билета</param>
        public Passenger(string fullName, string ticketNumber)
        {
            _fullName = fullName;
            _ticketNumber = ticketNumber;
            _assignedSeat = null; // Изначально место не назначено
        }

        // Свойства для доступа к атрибутам
        public string FullName 
        { 
            get { return _fullName; } 
        }

        public string TicketNumber 
        { 
            get { return _ticketNumber; } 
        }

        public Seat AssignedSeat 
        { 
            get { return _assignedSeat; } 
            set { _assignedSeat = value; } 
        }

        /// <summary>
        /// Проверка, есть ли у пассажира назначенное место
        /// </summary>
        public bool HasSeatAssigned
        {
            get { return _assignedSeat != null; }
        }

        /// <summary>
        /// Строковое представление пассажира
        /// </summary>
        public override string ToString()
        {
            string seatInfo = HasSeatAssigned ? $"Место: {_assignedSeat.SeatNumber}" : "Место не назначено";
            return $"Пассажир: {_fullName}, Билет: {_ticketNumber}, {seatInfo}";
        }
    }
}