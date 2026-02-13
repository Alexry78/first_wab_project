using System;

namespace GameProject
{
    /// <summary>
    /// Класс NPC (неигровой персонаж), наследующий GameEntity
    /// </summary>
    public class NPC : GameEntity
    {
        // Дополнительные поля NPC
        private string _dialogue;
        private string _role;

        /// <summary>
        /// Конструктор NPC
        /// </summary>
        public NPC(string name, int health, string role, string dialogue) : base(name, health)
        {
            _role = role;
            _dialogue = dialogue;
        }

        /// <summary>
        /// Свойство роли NPC
        /// </summary>
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        /// <summary>
        /// Свойство диалога
        /// </summary>
        public string Dialogue
        {
            get { return _dialogue; }
            set { _dialogue = value; }
        }

        /// <summary>
        /// Переопределение метода действия
        /// </summary>
        public override string Act()
        {
            if (!IsAlive())
                return $"{Name} мертв и больше ничего не скажет.";

            if (Health < 10)
                return $"{Name} ({_role}) в панике: 'Помогите! Я ранен!'";
            
            return $"{Name} ({_role}) занимается своими делами.";
        }

        /// <summary>
        /// Специфический метод NPC - разговор
        /// </summary>
        public void Talk()
        {
            if (!IsAlive())
            {
                Console.WriteLine($"{Name} мертв и не может говорить.");
                return;
            }

            Console.WriteLine($"{Name} ({_role}): \"{_dialogue}\"");
        }

        /// <summary>
        /// Переопределение ToString
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $" [Роль: {_role}]";
        }
    }
}