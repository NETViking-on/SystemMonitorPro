using System.Diagnostics;

namespace SystemMonitorPro.Models
{
   
    public class PerformanceService : IDisposable
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _diskRead;
        private readonly PerformanceCounter _diskWrite;

        public float TotalRamGb { get; private set; }

        public PerformanceService()
        {
            _cpuCounter = new PerformanceCounter(
                "Processor", "% Processor Time", "_Total");

            _diskRead = new PerformanceCounter(
                "PhysicalDisk", "Disk Read Bytes/sec", "_Total");

            _diskWrite = new PerformanceCounter(
                "PhysicalDisk", "Disk Write Bytes/sec", "_Total");

           
            _cpuCounter.NextValue();
            _diskRead.NextValue();
            _diskWrite.NextValue();

            TotalRamGb = GetTotalRamGb();
        }

        public PerformanceData Collect()
        {
            float cpuLoad = _cpuCounter.NextValue();
            float diskReadMbs = _diskRead.NextValue() / 1024f / 1024f;
            float diskWriteMbs = _diskWrite.NextValue() / 1024f / 1024f;

            var (usedGb, percent) = GetRamInfo();

            return new PerformanceData
            {
                CpuLoad = cpuLoad,
                RamUsedGb = usedGb,
                RamTotalGb = TotalRamGb,
                RamPercent = percent,
                DiskReadMbs = diskReadMbs,
                DiskWriteMbs = diskWriteMbs,
            };
        }

        private static float GetTotalRamGb()
        {
            var info = GC.GetGCMemoryInfo();
            return (float)(info.TotalAvailableMemoryBytes +
                           GC.GetTotalMemory(false)) / 1024f / 1024f / 1024f;
        }

        private static (float usedGb, float percent) GetRamInfo()
        {
           
            using var counter = new PerformanceCounter(
                "Memory", "Available MBytes");
            float availableMb = counter.NextValue();

            
            var info = GC.GetGCMemoryInfo();
            float totalGb = (float)info.TotalAvailableMemoryBytes
                            / 1024f / 1024f / 1024f;

            
            if (totalGb < 0.1f) totalGb = 8f;

            float usedGb = totalGb - (availableMb / 1024f);
            float percent = (usedGb / totalGb) * 100f;

            return (usedGb, percent);
        }

        public void Dispose()
        {
            _cpuCounter.Dispose();
            _diskRead.Dispose();
            _diskWrite.Dispose();
        }
    }
}