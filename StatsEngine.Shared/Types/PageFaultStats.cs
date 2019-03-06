using System;

namespace StatsEngine.Shared
{
    public class PageFaultStats : MachineStat
    {
        public PageFaultStats() : base() { }

        public uint LastPageFaultsPerSecond { get; set; }

        public uint LastPageReadsFaultedPerSecond { get; set; }

        public string LogMessage { get; set; }

        public override string ToLogString()
        {
            return String.Format("[{0}] Page faults per sec: [reads: {1}, faults: {2}]", TimeStamp, LastPageFaultsPerSecond, LastPageReadsFaultedPerSecond);
        }

    }
}
