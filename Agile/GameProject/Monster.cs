using System;

namespace GameProject
{
    /// <summary>
    /// Класс монстра, наследующий GameEntity
    /// </summary>
    public class Monster : GameEntity
    {
        // Дополнительные поля монстра
        private string _monsterType;
        private int _damage;

        /// <summary>
        /// Конструктор монстра
        /// </summary>
        public Monster(string name, int health, string monsterType, int damage) : base(name, health)
        {
            _monsterType = monsterType;
            _damage = damage;
        }

        /// <summary>
        /// Свойство типа монстра
        /// </summary>
        public string MonsterType
        {
            get { return _monsterType; }
            set { _monsterType = value; }
        }

        /// <summary>
        /// Свойство урона монстра
        /// </summary>
        public int Damage
        {
            get { return _damage; }
            set 
            { 
                if (value >= 0)
                    _damage = value;
            }
        }

        /// <summary>
        /// Переопределение метода действия
        /// </summary>
        public override string Act()
        {
            if (!IsAlive())
                return $"{Name} повержен и больше не двигается.";

            if (Health < 30)
                return $"{Name} ({_monsterType}) рычит от боли и пятится назад!";
            
            return $"{Name} ({_monsterType}) рычит и ищет жертву! (Урон: {_damage})";
        }

        /// <summary>
        /// Метод атаки монстра
        /// </summary>
        public void Attack(GameEntity target)
        {
            if (!IsAlive())
            {
                Console.WriteLine($"{Name} мертв и не может атаковать!");
                return;
            }

            Console.WriteLine($"{Name} атакует {target.Name} нанося {_damage} урона!");
            target.TakeDamage(_damage);
        }

        /// <summary>
        /// Переопределение ToString
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $" [Тип: {_monsterType}, Урон: {_damage}]";
        }
    }
}