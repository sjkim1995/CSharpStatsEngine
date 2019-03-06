using System;
using StatsEngine.Persistence;
using StatsEngine.Shared;

namespace StatsEngine.Logging
{
    class PageFaultLogger : MachineStatLogger
    {

        PageFaultHelper pfHelper;

        public PageFaultLogger(TimeSpan logFrequency, StatsBuffer<MachineStat> buf) : base(logFrequency, buf)
        {
            pfHelper = new PageFaultHelper(logFrequency);
        } 

        public override MachineStat GetStat()
        {
            // Update counters
            string logMessage = pfHelper.Log();
            return new PageFaultStats
            {
                TimeStamp = DateTimeOffset.UtcNow,
                LastPageFaultsPerSecond = PageFaultHelper.LastPageFaultsPerSecond,
                LastPageReadsFaultedPerSecond = PageFaultHelper.LastPageReadsFaultedPerSecond,
                LogMessage = logMessage
            };

        }
    }
}
