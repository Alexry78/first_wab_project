using System;

namespace ApplianceProject
{
    /// <summary>
    /// Класс холодильника, наследуется от Appliance
    /// </summary>
    public class Refrigerator : Appliance
    {
        // Дополнительный атрибут для холодильника
        private int _temperatureMode;    // Температурный режим в градусах Цельсия

        /// <summary>
        /// Конструктор холодильника
        /// </summary>
        /// <param name="applianceMake">Марка холодильника</param>
        /// <param name="consumptionWatts">Потребляемая мощность</param>
        /// <param name="temperatureMode">Температурный режим</param>
        public Refrigerator(string applianceMake, int consumptionWatts, int temperatureMode) 
            : base(applianceMake, consumptionWatts)
        {
            _temperatureMode = temperatureMode;
        }

        // Свойство для температурного режима
        public int TemperatureMode
        {
            get { return _temperatureMode; }
            set { _temperatureMode = value; }
        }

        /// <summary>
        /// Переопределенный метод для информации об использовании холодильника
        /// </summary>
        public override string UsageInfo()
        {
            return $"Холодильник марки {ApplianceMake} поддерживает температуру {_temperatureMode} градусов Цельсия.";
        }

        /// <summary>
        /// Переопределение ToString для холодильника
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $", Тип: Холодильник, Температура: {_temperatureMode}°C";
        }
    }
}