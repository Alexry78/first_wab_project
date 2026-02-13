using System;

namespace ApplianceProject
{
    /// <summary>
    /// Класс стиральной машины, наследуется от Appliance
    /// </summary>
    public class WashingMachine : Appliance
    {
        // Дополнительный атрибут для стиральной машины
        private string _washCycle;    // Цикл стирки

        /// <summary>
        /// Конструктор стиральной машины
        /// </summary>
        /// <param name="applianceMake">Марка стиральной машины</param>
        /// <param name="consumptionWatts">Потребляемая мощность</param>
        /// <param name="washCycle">Цикл стирки</param>
        public WashingMachine(string applianceMake, int consumptionWatts, string washCycle) 
            : base(applianceMake, consumptionWatts)
        {
            _washCycle = washCycle;
        }

        // Свойство для цикла стирки
        public string WashCycle
        {
            get { return _washCycle; }
            set { _washCycle = value; }
        }

        /// <summary>
        /// Переопределенный метод для информации об использовании стиральной машины
        /// </summary>
        public override string UsageInfo()
        {
            return $"Стиральная машина марки {ApplianceMake} осуществляет стирку в цикле \"{_washCycle}\".";
        }

        /// <summary>
        /// Переопределение ToString для стиральной машины
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $", Тип: Стиральная машина, Цикл стирки: {_washCycle}";
        }
    }
}