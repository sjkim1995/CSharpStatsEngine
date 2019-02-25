using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Diagnostics;
using CircularBuffer;

namespace StatsEngine.Loggers
{

    public struct BandwidthStats
    {
        public DateTimeOffset IntervalStart { get; set; }

        public DateTimeOffset IntervalEnd { get; set; }

        public long WriteDelta { get; set; }

        public long ReadDelta { get; set; }

        public double MbitsReadPerSecond { get; set; }

        public double MbitsWritePerSecond { get; set; }

    }

    class BandwidthLogger : IStatsEngineLogger<BandwidthStats>
    {
        private TimeSpan _logFrequency;
        private bool _writeToConsole;

        private bool _disposed;
        private long _previousReadBytes;
        private long _previousWriteBytes;
        DateTimeOffset _previousComputeTime;

        public BandwidthLogger(TimeSpan logFrequency, bool writeToConsole = false)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            _writeToConsole = writeToConsole;

            _previousComputeTime = DateTimeOffset.UtcNow;
            GetNetworkUsage(out _previousReadBytes, out _previousWriteBytes);
        }

        public async void StartLogging(CircularBuffer<BandwidthStats> buf)
        {
            try
            {
                while (!_disposed)
                {
                    await Task.Delay(_logFrequency);

                    var stat = GetStat();

                    LogStat(buf, stat);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public BandwidthStats GetStat()
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

        public void LogStat(CircularBuffer<BandwidthStats> buf, BandwidthStats stat)
        {
            buf.PushFront(stat);

            if (_writeToConsole)
            {
                Console.WriteLine("[{0}] BandWidth Usage ==> READ: {1} MBits/Sec, WRITE: {2} MBits/Sec",
                                DateTimeOffset.UtcNow.ToString("u"),
                                Math.Round(stat.MbitsReadPerSecond, 2),
                                Math.Round(stat.MbitsWritePerSecond, 2)
                                );
            }
            
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