using System;

namespace ServerMonitoring
{
    public class ServerContext
    {
        private int _cpuLoad;

        public ServerContext(int cpuLoad)
        {
            CpuLoad = cpuLoad;
            AlertSent = false;
            Restarted = false;
        }

        public int CpuLoad
        {
            get { return _cpuLoad; }
            set 
            { 
                _cpuLoad = Math.Clamp(value, 0, 100);
                Console.WriteLine($"  CPU нагрузка изменена: {_cpuLoad}%");
            }
        }

        public bool AlertSent { get; set; }
        public bool Restarted { get; set; }

        public override string ToString()
        {
            return $"CPU: {CpuLoad}% | Alert: {(AlertSent ? "Да" : "Нет")} | Restart: {(Restarted ? "Да" : "Нет")}";
        }
    }
}