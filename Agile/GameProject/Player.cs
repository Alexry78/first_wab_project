using System;

namespace GameProject
{
    /// <summary>
    /// Класс игрока, наследующий GameEntity
    /// </summary>
    public class Player : GameEntity
    {
        // Дополнительные поля игрока
        private int _experience;
        private int _level;

        /// <summary>
        /// Конструктор игрока
        /// </summary>
        public Player(string name, int health) : base(name, health)
        {
            _experience = 0;
            _level = 1;
        }

        /// <summary>
        /// Свойство опыта
        /// </summary>
        public int Experience
        {
            get { return _experience; }
            private set 
            { 
                _experience = value;
                CheckLevelUp();
            }
        }

        /// <summary>
        /// Свойство уровня
        /// </summary>
        public int Level
        {
            get { return _level; }
            private set { _level = value; }
        }

        /// <<summary>
        /// Метод получения опыта
        /// </summary>
        public void GainExperience(int exp)
        {
            if (exp > 0)
            {
                Experience += exp;
                Console.WriteLine($"{Name} получает {exp} опыта! Текущий опыт: {Experience}");
            }
        }

        /// <summary>
        /// Проверка повышения уровня
        /// </summary>
        private void CheckLevelUp()
        {
            int newLevel = 1 + (_experience / 100);
            if (newLevel > _level)
            {
                _level = newLevel;
                Health += 20; // Бонус здоровья за уровень
                Console.WriteLine($"Поздравляем! {Name} достиг {_level} уровня! Здоровье увеличено до {Health}");
            }
        }

        /// <summary>
        /// Переопределение метода действия
        /// </summary>
        public override string Act()
        {
            if (!IsAlive())
                return $"{Name} мертв и не может действовать.";
            
            return $"{Name} (Уровень {_level}) исследует местность в поисках приключений!";
        }

        /// <summary>
        /// Специфический метод игрока - атака
        /// </summary>
        public void Attack(GameEntity target)
        {
            if (!IsAlive())
            {
                Console.WriteLine($"{Name} мертв и не может атаковать!");
                return;
            }

            if (!target.IsAlive())
            {
                Console.WriteLine($"{target.Name} уже мертв!");
                return;
            }

            int damage = 10 + (_level * 2);
            Console.WriteLine($"{Name} атакует {target.Name} нанося {damage} урона!");
            target.TakeDamage(damage);

            if (!target.IsAlive())
            {
                Console.WriteLine($"{Name} победил {target.Name}!");
                GainExperience(50);
            }
        }
    }
}