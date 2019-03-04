using System;

namespace StatsEngine.Shared
{
    public class BandwidthStats : IMachineStat
    {
        public DateTimeOffset IntervalStart { get; set; }

        // Interval end
        public DateTimeOffset IntervalEnd { get; set; }

        public long WriteDelta { get; set; }

        public long ReadDelta { get; set; }

        public double MbitsReadPerSecond { get; set; }

        public double MbitsWritePerSecond { get; set; }

        public string ToLogString()
        {
            return string.Format("[{0}] BandWidth Usage ==> READ: {1} MBits/Sec, WRITE: {2} MBits/Sec",
                            IntervalEnd.ToString("u"),
                            Math.Round(MbitsReadPerSecond, 2),
                            Math.Round(MbitsWritePerSecond, 2));
        }

    }
}
