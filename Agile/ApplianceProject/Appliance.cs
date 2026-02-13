using System;

namespace ApplianceProject
{
    /// <summary>
    /// Базовый класс для бытовой техники
    /// </summary>
    public class Appliance
    {
        // Атрибуты базового класса
        private string _applianceMake;        // Марка бытовой техники
        private int _consumptionWatts;         // Потребляемая мощность в ваттах

        /// <summary>
        /// Конструктор базового класса
        /// </summary>
        /// <param name="applianceMake">Марка техники</param>
        /// <param name="consumptionWatts">Потребляемая мощность (Вт)</param>
        public Appliance(string applianceMake, int consumptionWatts)
        {
            _applianceMake = applianceMake;
            _consumptionWatts = consumptionWatts;
        }

        // Свойства для доступа к атрибутам
        public string ApplianceMake
        {
            get { return _applianceMake; }
            set { _applianceMake = value; }
        }

        public int ConsumptionWatts
        {
            get { return _consumptionWatts; }
            set 
            { 
                if (value > 0)
                    _consumptionWatts = value;
                else
                    throw new ArgumentException("Мощность должна быть положительным числом");
            }
        }

        /// <summary>
        /// Виртуальный метод для получения информации об использовании
        /// </summary>
        public virtual string UsageInfo()
        {
            return $"Бытовая техника марки {_applianceMake} потребляет {_consumptionWatts} Вт";
        }

        /// <summary>
        /// Переопределение ToString()
        /// </summary>
        public override string ToString()
        {
            return $"Прибор: {_applianceMake}, Мощность: {_consumptionWatts} Вт";
        }
    }
}