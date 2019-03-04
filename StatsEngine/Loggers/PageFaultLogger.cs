using System;
using StatsEngine.Persistence;
using StatsEngine.Shared;
using System.Runtime.InteropServices;

namespace StatsEngine.Logging
{
    class PageFaultLogger : StatsEngineLogger
    {

        PageFaultHelper pfHelper;

        public PageFaultLogger(TimeSpan logFrequency, StatsBuffer<IMachineStat> buf) : base(logFrequency, buf)
        {
            pfHelper = new PageFaultHelper(logFrequency);
        } 

        public override IMachineStat GetStat()
        {
            pfHelper.Log();
            return new PageFaultStats
            {
                TimeStamp = DateTimeOffset.UtcNow,
                LastHardPageFaultsPerSecond = PageFaultHelper.LastPageFaultsPerSecond,
                PageReadsFaulted = PageFaultHelper.PageReadsFaulted,
                PagesFaulted = PageFaultHelper.PagesFaulted
            };

        }
    }
}
