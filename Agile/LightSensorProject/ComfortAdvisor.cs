using System;

namespace LightSensorProject
{

    public class ComfortAdvisor
    {
        private int _lowLightCount = 0;  
        private int _totalMeasurements = 0;

  
        public ComfortAdvisor()
        {
            Console.WriteLine("[Advisor] Создан экземпляр ComfortAdvisor");
        }


        public void OnLightLevelChanged(LightSensor sender, int lux)
        {
            _totalMeasurements++;
            
            if (lux < 200)
            {
                _lowLightCount++;
                Console.WriteLine($"[Advisor] Значение {lux} лк — темновато (ниже 200 лк)");
            }
        }


        public void Report()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("[Advisor] СТАТИСТИКА ОСВЕЩЁННОСТИ:");
            Console.WriteLine($"[Advisor] Всего измерений: {_totalMeasurements}");
            Console.WriteLine($"[Advisor] Низкая освещённость (<200 лк) встречалась {_lowLightCount} раз");
            Console.WriteLine(new string('=', 50));
        }
    }
}