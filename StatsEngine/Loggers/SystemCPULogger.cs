using StatsEngine.Persistence;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StatsEngine.Types;


namespace StatsEngine.Logging
{
    class SystemCPULogger : IStatsEngineLogger
    {
        private TimeSpan _logFrequency;
        private StatsBuffer<SystemCPUStats> _buf;

        private bool _disposed;
        static object staticLock = new object();
        static volatile PerformanceCounter _cpu;
        static volatile bool _disabled;

        public SystemCPULogger(TimeSpan logFrequency, StatsBuffer<SystemCPUStats> buf)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            _buf = buf;
        }

        public async void StartLogging()
        {
            try
            {
                while (!_disposed)
                {
                    await Task.Delay(_logFrequency);

                    var stat = GetStat();

                    LogStat(stat);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public virtual void LogStat(SystemCPUStats stat)
        {
            _buf.AddStat(stat);
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
