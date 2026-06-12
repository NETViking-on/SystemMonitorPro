using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitorPro.Models
{
    public class PerformanceData
    {
        public float CpuLoad { get; set; }  // 0-100 %
        public float RamUsedGb { get; set; }  // в ГБ
        public float RamTotalGb { get; set; }  // в ГБ
        public float RamPercent { get; set; }  // 0-100 %
        public float DiskReadMbs { get; set; }  // МБ/с
        public float DiskWriteMbs { get; set; }  // МБ/с
    }
}
