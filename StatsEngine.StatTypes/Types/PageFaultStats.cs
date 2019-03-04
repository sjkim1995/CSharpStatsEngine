using System;

namespace StatsEngine.Shared
{
    public class PageFaultStats : IMachineStat
    {
        public DateTimeOffset TimeStamp { get; set; }

        public uint LastHardPageFaultsPerSecond { get; set; }

        public uint PagesFaulted { get; set; }

        public uint PageReadsFaulted { get; set; }

        public string ToLogString()
        {
            return String.Format("[{0}] Page faults per sec: [reads: {1}, faults: {2}]", TimeStamp, PageReadsFaulted, PagesFaulted);
        }

    }
}
