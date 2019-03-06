using System;

namespace StatsEngine.Shared
{
    public class BandwidthStats : MachineStat
    {
        public BandwidthStats() : base() { }

        // IntervalEnd is the TimeStamp value on the base class
        public DateTimeOffset IntervalStart { get; set; }

        public long WriteDelta { get; set; }

        public long ReadDelta { get; set; }

        public double MbitsReadPerSecond { get; set; }

        public double MbitsWritePerSecond { get; set; }

        public override string ToLogString()
        {
            return string.Format("[{0}] BandWidth Usage ==> READ: {1} MBits/Sec, WRITE: {2} MBits/Sec",
                            TimeStamp.ToString("u"),
                            Math.Round(MbitsReadPerSecond, 2),
                            Math.Round(MbitsWritePerSecond, 2));
        }

    }
}
