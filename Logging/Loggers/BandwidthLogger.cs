using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Diagnostics;
using StatsEngine.Persistence;
using StatsEngine.Shared;

namespace StatsEngine.Logging
{
    class BandwidthLogger : MachineStatLogger
    {
        private long _previousReadBytes;
        private long _previousWriteBytes;
        DateTimeOffset _previousComputeTime;

        public BandwidthLogger(TimeSpan logFrequency, StatsBuffer<MachineStat> buf) : base(logFrequency, buf)
        {
            _previousComputeTime = DateTimeOffset.UtcNow;
            GetNetworkUsage(out _previousReadBytes, out _previousWriteBytes);
        }

        public override MachineStat GetStat()
        {
            const long bitsPerByte = 8;
            const double oneMeg = 1024 * 1024;

            GetNetworkUsage(out long bytesRead, out long bytesWrite);

            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            TimeSpan elapsed = currentTime - _previousComputeTime;

            long readDelta = (bytesRead - _previousReadBytes);
            long writeDelta = (bytesWrite - _previousWriteBytes);

            var stat = new BandwidthStats
            {
                ReadDelta = readDelta,
                WriteDelta = writeDelta,
                IntervalStart = _previousComputeTime,
                TimeStamp = currentTime
            };

            _previousReadBytes = bytesRead;
            _previousWriteBytes = bytesWrite;
            _previousComputeTime = currentTime;

            double mbitsReadPerSecond = readDelta <= 0 ? 0 : ((readDelta * bitsPerByte) / oneMeg) / elapsed.TotalSeconds;
            double mbitsWritePerSecond = writeDelta <= 0 ? 0 : ((writeDelta * bitsPerByte) / oneMeg) / elapsed.TotalSeconds;

            stat.MbitsReadPerSecond = mbitsReadPerSecond;
            stat.MbitsWritePerSecond = mbitsWritePerSecond;

            return stat;
        }

        private static void GetNetworkUsage(out long bytesRead, out long bytesWrite)
        {
            bytesRead = 0L;
            bytesWrite = 0L;

            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var nic in nics)
                {
                    long nicbytesRead = nic.GetIPStatistics().BytesReceived;
                    long nicbytesWrite = nic.GetIPStatistics().BytesSent;
                    bytesRead += nicbytesRead;
                    bytesWrite += nicbytesWrite;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}