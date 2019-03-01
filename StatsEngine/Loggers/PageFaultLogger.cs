using System;
using StatsEngine.Persistence;
using StatsEngine.Shared.Types;
using System.Runtime.InteropServices;

namespace StatsEngine.Logging
{
    class PageFaultLogger : StatsEngineLogger
    {

        PageFaultHelper pfHelper;

        public PageFaultLogger(TimeSpan logFrequency, StatsBuffer<IMachineStat> buf) : base(logFrequency, buf)
        {
            // OSX and Linux
            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotImplementedException();
            }
            pfHelper = new PageFaultHelper(false);
        } 

        public override IMachineStat GetStat()
        {
            pfHelper.Log();
            return new PageFaultStats
            {
                TimeStamp = DateTimeOffset.UtcNow,
                LastPageFaultsPerSecond = PageFaultHelper.LastPageFaultsPerSecond
            };

        }
    }
}
