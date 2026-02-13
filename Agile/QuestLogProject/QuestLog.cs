using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuestLogProject
{
       public class QuestLog : IEnumerable<Quest>
    {
          private readonly List<Quest> _quests;
        private readonly Dictionary<string, Quest> _byId;

           public QuestLog()
        {
            _quests = new List<Quest>();
            _byId = new Dictionary<string, Quest>();
        }

        public int Count => _quests.Count;

        public Quest this[int index]
        {
            get
            {
                if (index < 0 || index >= _quests.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), 
                        $"Индекс {index} вне диапазона (0-{_quests.Count - 1})");
                
                return _quests[index];
            }
        }

        public Quest this[string id]
        {
            get
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id));
                
                if (!_byId.ContainsKey(id))
                    throw new KeyNotFoundException($"Квест с ID '{id}' не найден");
                
                return _byId[id];
            }
        }

         public void Add(Quest quest)
        {
            if (quest == null)
                throw new ArgumentNullException(nameof(quest));

             if (_byId.ContainsKey(quest.Id))
                throw new ArgumentException($"Квест с ID '{quest.Id}' уже существует");

            _quests.Add(quest);
            _byId.Add(quest.Id, quest);
        }

         public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _quests.Count)
                return false;

            Quest quest = _quests[index];
            
               _quests.RemoveAt(index);
            _byId.Remove(quest.Id);
            
            return true;
        }
   public bool RemoveById(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (!_byId.TryGetValue(id, out Quest? quest))
                return false;

             _quests.Remove(quest);
            
              _byId.Remove(id);
            
            return true;
        }


        public IEnumerable<Quest> EnumerateByDifficulty(Difficulty minDifficulty)
        {
            foreach (var quest in _quests)
            {
                if (quest.Difficulty >= minDifficulty)
                {
                    yield return quest;
                }
            }
        }


        public IEnumerator<Quest> GetEnumerator()
        {
            return _quests.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public bool ContainsId(string id)
        {
            return id != null && _byId.ContainsKey(id);
        }

        public bool TryGetQuest(string id, out Quest? quest)
        {
            if (id == null)
            {
                quest = null;
                return false;
            }
            
            return _byId.TryGetValue(id, out quest);
        }
    }
}