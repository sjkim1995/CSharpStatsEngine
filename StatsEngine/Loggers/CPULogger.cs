using CircularBuffer;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StatsEngine.Loggers
{
    public struct SystemCPUStats
    {
        public DateTime TimeStamp { get; set; }

        public double? CPU { get; set; }
    }

    class CPULogger : IStatsEngineLogger<SystemCPUStats>
    {
        private TimeSpan _logFrequency;
        private bool _writeToConsole;

        private bool _disposed;
        static object staticLock = new object();
        static volatile PerformanceCounter _cpu;
        static volatile bool _disabled;

        public CPULogger(TimeSpan logFrequency, bool writeToConsole = false)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            _writeToConsole = writeToConsole;
        }

        public async void StartLogging(CircularBuffer<SystemCPUStats> buf)
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

        public virtual void LogStat(CircularBuffer<SystemCPUStats> buf, SystemCPUStats stat)
        {
            buf.PushFront(stat);

            if (_writeToConsole)
            {
                string message = string.Format("[{0}] CPU: {1}%", stat.TimeStamp, stat.CPU);
                Console.WriteLine(message);
            }

        }

        public SystemCPUStats GetStat()
        {
            float systemCPU;
            double? cpu = null;

            if (TryGetSystemCPU(out systemCPU))
            {
                cpu = Math.Round(systemCPU, 2);
            }

            return new SystemCPUStats
            {
                TimeStamp = DateTime.UtcNow,
                CPU = cpu
            };
        }

        private bool TryGetSystemCPU(out float value)
        {
            value = -1;

            try
            {
                if (!_disabled && _cpu == null)
                {
                    lock (staticLock)
                    {
                        if (_cpu == null)
                        {
                            _cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                            // First call always returns 0, so get that out of the way.
                            _cpu.NextValue();
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Some environments don't allow access to Performance Counters, so stop trying.
                _disabled = true;
            }
            catch (Exception e)
            {
                // this shouldn't happen, but just being safe...
                Trace.WriteLine(e);
            }

            if (!_disabled && _cpu != null)
            {
                value = _cpu.NextValue();
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
