using System;

namespace GameProject
{
    /// <summary>
    /// Базовый класс для всех игровых сущностей
    /// </summary>
    public class GameEntity
    {
        // Приватные поля
        private string _name;
        private int _health;

        /// <summary>
        /// Конструктор базового класса
        /// </summary>
        /// <param name="name">Имя сущности</param>
        /// <param name="health">Здоровье сущности</param>
        public GameEntity(string name, int health)
        {
            Name = name;        // Используем свойство для валидации
            Health = health;    // Используем свойство для валидации
        }

        /// <summary>
        /// Свойство Name с валидацией (не пустая строка)
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя не может быть пустым");
                _name = value;
            }
        }

        /// <summary>
        /// Свойство Health с валидацией (≥ 0)
        /// </summary>
        public int Health
        {
            get { return _health; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Здоровье не может быть отрицательным");
                _health = value;
            }
        }

        /// <summary>
        /// Невиртуальный метод получения урона
        /// </summary>
        /// <param name="dmg">Величина урона</param>
        public void TakeDamage(int dmg)
        {
            if (dmg < 0)
            {
                Console.WriteLine($"{Name}: отрицательный урон? Это лечение!");
                Health += Math.Abs(dmg); // Лечим, если передан отрицательный урон
            }
            else
            {
                Console.WriteLine($"{Name} получает {dmg} урона!");
                Health = Math.Max(0, Health - dmg);
            }
        }

        /// <summary>
        /// Невиртуальный метод проверки жив ли объект
        /// </summary>
        /// <returns>true если здоровье > 0</returns>
        public bool IsAlive()
        {
            return Health > 0;
        }

        /// <summary>
        /// Виртуальный метод действия
        /// </summary>
        /// <returns>Строка с описанием действия</returns>
        public virtual string Act()
        {
            return $"{Name} бездействует.";
        }

        /// <summary>
        /// Переопределение ToString
        /// </summary>
        public override string ToString()
        {
            string status = IsAlive() ? $"Жив (Здоровье: {Health})" : "Мертв";
            return $"{Name} [{status}]";
        }
    }
}