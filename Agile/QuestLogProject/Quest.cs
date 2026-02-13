using System;
using System.Collections.Generic;

namespace QuestLogProject
{

    public class Quest
    {
          private readonly List<Objective> _objectives;


        /// <param name="id">Уникальный идентификатор</param>
        /// <param name="title">Название квеста</param>
        /// <param name="difficulty">Сложность</param>
        /// <param name="objectives">Список целей (опционально)</param>
        public Quest(string id, string title, Difficulty difficulty, IEnumerable<Objective>? objectives = null)
        {

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID квеста не может быть пустым", nameof(id));
            
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название квеста не может быть пустым", nameof(title));

            Id = id;
            Title = title;
            Difficulty = difficulty;
            
             _objectives = new List<Objective>();
            
            if (objectives != null)
            {
                foreach (var obj in objectives)
                {
                    AddObjective(obj);
                }
            }
        }

        public string Id { get; }

        public string Title { get; }

        public Difficulty Difficulty { get; }


        public IReadOnlyList<Objective> Objectives => _objectives.AsReadOnly();

        private void AddObjective(Objective objective)
        {
            if (objective == null)
                throw new ArgumentNullException(nameof(objective));
            
            _objectives.Add(objective);
        }


        public override string ToString()
        {
            return $"[{Id}] {Title} (Сложность: {Difficulty}, Целей: {_objectives.Count})";
        }
    }
}