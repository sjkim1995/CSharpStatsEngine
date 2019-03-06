using System;

namespace StatsEngine.Shared
{
    public class CPUStats : MachineStat
    {
        public CPUStats() : base() { }

        public double? CPU { get; set; }

        public override string ToLogString()
        {
            return string.Format("[{0}] CPU: {1}%", TimeStamp.ToString("u"), CPU);
        }
    }

}
