using System;

namespace StatsEngine.Shared
{
    public class CPUStats : IMachineStat
    {
        public DateTimeOffset TimeStamp { get; set; }

        public double? CPU { get; set; }

        public string ToLogString()
        {
            return string.Format("[{0}] CPU: {1}%", TimeStamp.ToString("u"), CPU);
        }
    }

}
