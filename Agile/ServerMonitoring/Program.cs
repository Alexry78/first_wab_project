using System;

namespace ServerMonitoring
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА МОНИТОРИНГА СЕРВЕРА ===\n");

            var pipeline = new MonitorPipeline();
            
  
            MonitorHandler sendAlert = Handlers.SendAlert;
            MonitorHandler restartService = Handlers.RestartService;
            MonitorHandler reduceLoad = Handlers.CreateReduceLoadHandler(10);

            Console.WriteLine(">> Добавление обработчиков:");
            pipeline.AddHandler(sendAlert);
            pipeline.AddHandler(restartService);
            pipeline.AddHandler(reduceLoad);


            var context1 = new ServerContext(96);
            pipeline.Run(context1);


            Console.WriteLine("\n>> Удаление обработчика RestartService:");
            pipeline.RemoveHandler(restartService);


            var context2 = new ServerContext(98);
            pipeline.Run(context2);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}