using System;

namespace ServerMonitoring
{
    public static class Handlers
    {
        public static void SendAlert(ServerContext context)
        {
            Console.WriteLine("  SendAlert: Проверка...");
            
            if (context.CpuLoad > 80 && !context.AlertSent)
            {
                context.AlertSent = true;
                Console.WriteLine($"  SendAlert: ОПОВЕЩЕНИЕ! CPU = {context.CpuLoad}% > 80%");
            }
            else
            {
                Console.WriteLine($"  SendAlert: Условие не выполнено (CPU={context.CpuLoad}%, AlertSent={context.AlertSent})");
            }
        }

        public static void RestartService(ServerContext context)
        {
            Console.WriteLine("  RestartService: Проверка...");
            
            if (context.CpuLoad > 95 && !context.Restarted)
            {
                context.Restarted = true;
                Console.WriteLine($"  RestartService: ПЕРЕЗАГРУЗКА! CPU = {context.CpuLoad}% > 95%");
                context.CpuLoad = 30;
            }
            else
            {
                Console.WriteLine($"  RestartService: Условие не выполнено (CPU={context.CpuLoad}%, Restarted={context.Restarted})");
            }
        }

        public static MonitorHandler CreateReduceLoadHandler(int percent)
        {
            return context =>
            {
                int oldLoad = context.CpuLoad;
                context.CpuLoad -= percent;
                Console.WriteLine($"  ReduceLoad: Снижение на {percent}%: {oldLoad}% -> {context.CpuLoad}%");
            };
        }
    }
}