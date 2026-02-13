using System;

namespace QuestLogProject
{

    public class Objective
    {

        /// <param name="code">Короткий код цели</param>
        /// <param name="description">Описание цели</param>
        /// <param name="requiredCount">Требуемое количество (>=1)</param>
        public Objective(string code, string description, int requiredCount = 1)
        {

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Код цели не может быть пустым", nameof(code));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Описание цели не может быть пустым", nameof(description));
            
            if (requiredCount < 1)
                throw new ArgumentException("Требуемое количество должно быть >= 1", nameof(requiredCount));

            Code = code;
            Description = description;
            RequiredCount = requiredCount;
        }

        public string Code { get; }

    
        public string Description { get; }

       
        public int RequiredCount { get; }

        public override string ToString()
        {
            return $"[{Code}] {Description} (x{RequiredCount})";
        }
    }
}