using System;
using System.Collections.Generic;

namespace AirplaneProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления посадкой в самолёт ===\n");

            // Создаём самолёт
            Airplane airplane = new Airplane("RA-12345", "Aeroflot", 120);
            Console.WriteLine(airplane);
            
            // Создаём пассажиров
            List<Passenger> passengers = new List<Passenger>
            {
                new Passenger("Иванов Иван Иванович", "TKT-001"),
                new Passenger("Петров Пётр Петрович", "TKT-002"),
                new Passenger("Сидорова Анна Сергеевна", "TKT-003"),
                new Passenger("Козлов Дмитрий Александрович", "TKT-004"),
                new Passenger("Смирнова Елена Владимировна", "TKT-005")
            };

            Console.WriteLine("\n=== Список пассажиров ===");
            foreach (var passenger in passengers)
            {
                Console.WriteLine(passenger);
            }

            Console.WriteLine("\n=== Процесс посадки ===");
            
            // Производим посадку пассажиров на разные места
            airplane.BoardPassenger(passengers[0], "1A");  // Первый класс
            airplane.BoardPassenger(passengers[1], "3B");  // Бизнес-класс
            airplane.BoardPassenger(passengers[2], "10C"); // Эконом-класс
            airplane.BoardPassenger(passengers[3], "15D"); // Эконом-класс
            
            // Попытка посадить пассажира на уже занятое место
            Console.WriteLine("\n=== Попытка занять уже занятое место ===");
            airplane.BoardPassenger(passengers[4], "1A");
            
            // Попытка посадить на несуществующее место
            Console.WriteLine("\n=== Попытка занять несуществующее место ===");
            airplane.BoardPassenger(passengers[4], "99Z");

            // Выводим информацию о самолёте
            airplane.DisplayOccupancyInfo();

            // Показываем информацию о нескольких местах
            Console.WriteLine("\n=== Информация о некоторых местах ===");
            string[] seatsToShow = { "1A", "3B", "10C", "20F" };
            foreach (string seatNumber in seatsToShow)
            {
                var seat = airplane.Seats.Find(s => s.SeatNumber == seatNumber);
                if (seat != null)
                {
                    Console.WriteLine(seat);
                }
            }

            // Высадка пассажира
            Console.WriteLine("\n=== Высадка пассажира с места 10C ===");
            airplane.DisembarkPassenger("10C");
            
            // Проверяем, что место освободилось
            var freedSeat = airplane.Seats.Find(s => s.SeatNumber == "10C");
            Console.WriteLine(freedSeat);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}