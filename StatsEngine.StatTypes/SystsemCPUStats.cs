using System;

namespace StatsEngine.Types
{
    public struct SystemCPUStats
    {
        public DateTimeOffset TimeStamp { get; set; }

        public double? CPU { get; set; }

        public string FormatToString()
        {
            return string.Format("[{0}] CPU: {1}%", TimeStamp.ToString("u"), CPU);
        }
    }

}
