using System;
using System.Collections.Generic;

namespace LightSensorProject
{

    public class LightSensor
    {

        public delegate void LightEventHandler(LightSensor sender, int lux);


        public event LightEventHandler? LightLevelChanged;


        private EventHandler<int>? _blindingLightHandlers;

        public event EventHandler<int> BlindingLightReached
        {
            add
            {
                _blindingLightHandlers += value;
                Console.WriteLine($"[Сенсор] Подписчик добавлен на событие BlindingLightReached: {value.Method.Name}");
            }
            remove
            {
                _blindingLightHandlers -= value;
                Console.WriteLine($"[Сенсор] Подписчик удалён с события BlindingLightReached: {value.Method.Name}");
            }
        }


        public void Start()
        {
            Console.WriteLine("[Сенсор] Начало измерений...\n");
            
            Random random = new Random();
            int measurements = random.Next(8, 13); 
            
            for (int i = 0; i < measurements; i++)
            {

                int lux = random.Next(50, 1001);
                
                Console.WriteLine($"[Сенсор] Измерение {i + 1}: {lux} лк");
                

                OnLightLevelChanged(lux);
                

                if (lux >= 800)
                {
                    OnBlindingLightReached(lux);
                }
                

                System.Threading.Thread.Sleep(500);
            }
            
            Console.WriteLine("\n[Сенсор] Измерения завершены.\n");
        }


        protected virtual void OnLightLevelChanged(int lux)
        {
            LightLevelChanged?.Invoke(this, lux);
        }


        protected virtual void OnBlindingLightReached(int lux)
        {
            _blindingLightHandlers?.Invoke(this, lux);
        }
    }
}