using System;

namespace ServerMonitoring
{
    public class MonitorPipeline
    {

        public MonitorHandler? MonitorHandler { get; set; }


        public MonitorPipeline()
        {
        }

        public void Run(ServerContext ctx)
        {
            Console.WriteLine($"\n=== Запуск конвейера обработчиков ===");
            Console.WriteLine($"Начальное состояние: {ctx}");
            
            if (MonitorHandler == null)
            {
                Console.WriteLine("Нет зарегистрированных обработчиков!");
                return;
            }

            MonitorHandler(ctx);
            
            Console.WriteLine($"Итоговое состояние: {ctx}");
        }

        public void AddHandler(MonitorHandler handler)
        {
            MonitorHandler += handler;
            Console.WriteLine($"Обработчик {handler.Method.Name} добавлен");
        }


        public void RemoveHandler(MonitorHandler handler)
        {
            MonitorHandler -= handler;
            Console.WriteLine($"Обработчик {handler.Method.Name} удален");
        }
    }
}