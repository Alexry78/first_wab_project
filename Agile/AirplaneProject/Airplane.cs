using System;
using System.Collections.Generic;
using System.Linq;

namespace AirplaneProject
{
    /// <summary>
    /// Класс, представляющий самолёт
    /// </summary>
    public class Airplane
    {
        // Атрибуты самолёта
        private string _registrationNumber;    // Регистрационный номер
        private string _airline;                // Авиакомпания
        private int _capacity;                   // Вместимость салона
        private List<Seat> _seats;               // Список мест в самолёте

        /// <summary>
        /// Конструктор самолёта
        /// </summary>
        /// <param name="registrationNumber">Регистрационный номер</param>
        /// <param name="airline">Авиакомпания</param>
        /// <param name="capacity">Вместимость</param>
        public Airplane(string registrationNumber, string airline, int capacity)
        {
            _registrationNumber = registrationNumber;
            _airline = airline;
            _capacity = capacity;
            _seats = new List<Seat>();
            
            // При создании самолёта генерируем места
            GenerateSeats();
        }

        // Свойства для доступа к атрибутам
        public string RegistrationNumber 
        { 
            get { return _registrationNumber; } 
        }

        public string Airline 
        { 
            get { return _airline; } 
        }

        public int Capacity 
        { 
            get { return _capacity; } 
        }

        public List<Seat> Seats 
        { 
            get { return _seats; } 
        }

        /// <summary>
        /// Генерация мест в самолёте
        /// </summary>
        private void GenerateSeats()
        {
            // Определяем количество мест по классам
            int businessSeats = (int)(_capacity * 0.2);  // 20% бизнес-класс
            int firstClassSeats = (int)(_capacity * 0.1); // 10% первый класс
            int economySeats = _capacity - businessSeats - firstClassSeats; // остальное эконом

            char[] rows = { 'A', 'B', 'C', 'D', 'E', 'F' };
            int seatCounter = 1;

            // Создаём места первого класса (ряды 1-2)
            for (int i = 1; i <= firstClassSeats / 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_seats.Count < firstClassSeats)
                    {
                        _seats.Add(new Seat($"{i}{rows[j]}", "First"));
                    }
                }
            }

            // Создаём места бизнес-класса (ряды 3-6)
            for (int i = 3; i <= 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (_seats.Count < firstClassSeats + businessSeats)
                    {
                        _seats.Add(new Seat($"{i}{rows[j]}", "Business"));
                    }
                }
            }

            // Создаём остальные места эконом-класса
            for (int i = 7; i <= 30; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (_seats.Count < _capacity)
                    {
                        _seats.Add(new Seat($"{i}{rows[j]}", "Economy"));
                    }
                }
            }
        }

        /// <summary>
        /// Посадка пассажира на указанное место
        /// </summary>
        public bool BoardPassenger(Passenger passenger, string seatNumber)
        {
            // Ищем указанное место
            Seat seat = _seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
            
            if (seat == null)
            {
                Console.WriteLine($"Место {seatNumber} не найдено");
                return false;
            }

            if (seat.IsOccupied)
            {
                Console.WriteLine($"Место {seatNumber} уже занято");
                return false;
            }

            // Назначаем пассажира на место
            seat.Passenger = passenger;
            passenger.AssignedSeat = seat;
            Console.WriteLine($"Пассажир {passenger.FullName} посажен на место {seatNumber}");
            return true;
        }

        /// <summary>
        /// Высадка пассажира с указанного места
        /// </summary>
        public bool DisembarkPassenger(string seatNumber)
        {
            Seat seat = _seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
            
            if (seat == null || !seat.IsOccupied)
            {
                Console.WriteLine($"Место {seatNumber} свободно или не найдено");
                return false;
            }

            string passengerName = seat.Passenger.FullName;
            seat.Passenger.AssignedSeat = null;
            seat.ClearSeat();
            Console.WriteLine($"Пассажир {passengerName} высажен с места {seatNumber}");
            return true;
        }

        /// <summary>
        /// Получение информации о занятых местах
        /// </summary>
        public void DisplayOccupancyInfo()
        {
            int occupiedSeats = _seats.Count(s => s.IsOccupied);
            Console.WriteLine($"\n=== Информация о самолёте {_registrationNumber} ===");
            Console.WriteLine($"Авиакомпания: {_airline}");
            Console.WriteLine($"Вместимость: {_capacity} мест");
            Console.WriteLine($"Занято мест: {occupiedSeats}");
            Console.WriteLine($"Свободно мест: {_capacity - occupiedSeats}");
            
            Console.WriteLine("\nРаспределение по классам:");
            var firstClass = _seats.Where(s => s.ServiceClass == "First");
            var businessClass = _seats.Where(s => s.ServiceClass == "Business");
            var economyClass = _seats.Where(s => s.ServiceClass == "Economy");
            
            Console.WriteLine($"Первый класс: {firstClass.Count(s => s.IsOccupied)}/{firstClass.Count()}");
            Console.WriteLine($"Бизнес-класс: {businessClass.Count(s => s.IsOccupied)}/{businessClass.Count()}");
            Console.WriteLine($"Эконом-класс: {economyClass.Count(s => s.IsOccupied)}/{economyClass.Count()}");
        }

        /// <summary>
        /// Строковое представление самолёта
        /// </summary>
        public override string ToString()
        {
            return $"Самолёт {_registrationNumber}, Авиакомпания: {_airline}, Вместимость: {_capacity} мест";
        }
    }
}