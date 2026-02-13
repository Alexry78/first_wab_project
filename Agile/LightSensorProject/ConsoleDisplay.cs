using System;

namespace LightSensorProject
{

    public class ConsoleDisplay
    {

        public ConsoleDisplay()
        {
            Console.WriteLine("[Display] Создан экземпляр ConsoleDisplay");
        }

 
        public void OnLightLevelChanged(LightSensor sender, int lux)
        {
            Console.WriteLine($"[Display] Освещённость: {lux} лк");
        }


        public void OnBlindingLightReached(object? sender, int lux)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Display] ВНИМАНИЕ! Слишком ярко: {lux} лк — прищурьтесь или снизьте яркость!");
            Console.ResetColor();
        }


        public void Unsubscribe(LightSensor sensor)
        {
            sensor.BlindingLightReached -= OnBlindingLightReached;
        }
    }
}