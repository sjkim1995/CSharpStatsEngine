using System;
using StatsEngine.Shared.Types;

namespace StatsEngine.Shared.Types
{
    public class PageFaultStats : IMachineStat
    {
        public DateTimeOffset TimeStamp { get; set; }

        public uint LastPageFaultsPerSecond { get; set; }

        public string ToLogString()
        {
            return String.Format("[{0}] {1} page faults per second", TimeStamp, LastPageFaultsPerSecond);
        }

    }
}
