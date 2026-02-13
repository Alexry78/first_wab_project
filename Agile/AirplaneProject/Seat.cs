using System;

namespace AirplaneProject
{
    /// <summary>
    /// Класс, представляющий место в самолёте
    /// </summary>
    public class Seat
    {
        // Атрибуты места
        private string _seatNumber;      // Номер кресла (например, "12A")
        private string _serviceClass;    // Класс обслуживания (Economy, Business, First)
        private Passenger _passenger;    // Пассажир на этом месте (null если свободно)

        /// <summary>
        /// Конструктор места
        /// </summary>
        /// <param name="seatNumber">Номер кресла</param>
        /// <param name="serviceClass">Класс обслуживания</param>
        public Seat(string seatNumber, string serviceClass)
        {
            _seatNumber = seatNumber;
            _serviceClass = serviceClass;
            _passenger = null; // Изначально место свободно
        }

        // Свойства для доступа к атрибутам
        public string SeatNumber 
        { 
            get { return _seatNumber; } 
        }

        public string ServiceClass 
        { 
            get { return _serviceClass; } 
        }

        public Passenger Passenger 
        { 
            get { return _passenger; } 
            set { _passenger = value; } 
        }

        /// <summary>
        /// Проверка, занято ли место
        /// </summary>
        public bool IsOccupied
        {
            get { return _passenger != null; }
        }

        /// <summary>
        /// Освободить место
        /// </summary>
        public void ClearSeat()
        {
            _passenger = null;
        }

        /// <summary>
        /// Строковое представление места
        /// </summary>
        public override string ToString()
        {
            string status = IsOccupied ? $"Занято: {_passenger.FullName}" : "Свободно";
            return $"Место {_seatNumber} ({_serviceClass} класс) - {status}";
        }
    }
}