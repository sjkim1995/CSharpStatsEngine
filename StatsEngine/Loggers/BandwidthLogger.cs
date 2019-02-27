using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Diagnostics;
using StatsEngine.Persistence;
using StatsEngine.Types;

namespace StatsEngine.Logging
{
    class BandwidthLogger : IStatsEngineLogger
    {
        private TimeSpan _logFrequency;
        private StatsBuffer<BandwidthStats> _buf;

        private bool _disposed;
        private long _previousReadBytes;
        private long _previousWriteBytes;
        DateTimeOffset _previousComputeTime;

        public BandwidthLogger(TimeSpan logFrequency, StatsBuffer<BandwidthStats> buf)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            _buf = buf; 

            _previousComputeTime = DateTimeOffset.UtcNow;
            GetNetworkUsage(out _previousReadBytes, out _previousWriteBytes);
        }

        public async void StartLogging()
        {
            try
            {
                while (!_disposed)
                {
                    await Task.Delay(_logFrequency);

                    var stat = GetCurrentStat();

                    LogStat(stat);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public BandwidthStats GetCurrentStat()
        {
            const long bitsPerByte = 8;
            const double oneMeg = 1024 * 1024;

            long bytesRead;
            long bytesWrite;
            GetNetworkUsage(out bytesRead, out bytesWrite);

            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            TimeSpan elapsed = currentTime - _previousComputeTime;

            long readDelta = (bytesRead - _previousReadBytes);
            long writeDelta = (bytesWrite - _previousWriteBytes);

            var stat = new BandwidthStats();
            stat.ReadDelta = readDelta;
            stat.WriteDelta = writeDelta;
            stat.IntervalStart = _previousComputeTime;
            stat.IntervalEnd = currentTime;

            _previousReadBytes = bytesRead;
            _previousWriteBytes = bytesWrite;
            _previousComputeTime = currentTime;

            double mbitsReadPerSecond = readDelta <= 0 ? 0 : ((readDelta * bitsPerByte) / oneMeg) / elapsed.TotalSeconds;
            double mbitsWritePerSecond = writeDelta <= 0 ? 0 : ((writeDelta * bitsPerByte) / oneMeg) / elapsed.TotalSeconds;

            stat.MbitsReadPerSecond = mbitsReadPerSecond;
            stat.MbitsWritePerSecond = mbitsWritePerSecond;

            return stat;
        }

        public void LogStat(BandwidthStats stat)
        {
            _buf.AddStat(stat);      
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

        public void Dispose()
        {
            _disposed = true;
        }
    }
}