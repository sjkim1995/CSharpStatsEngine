using StatsEngine.Persistence;
using System;
using System.Diagnostics;
using StatsEngine.Shared;


namespace StatsEngine.Logging
{
    class CPULogger : MachineStatLogger
    {
        public CPULogger(TimeSpan logFrequency, StatsBuffer<MachineStat> buf) : base(logFrequency, buf)
        {
        }

        static object staticLock = new object();
        static volatile PerformanceCounter _cpu;
        static volatile bool _disabled;

        public override MachineStat GetStat()
        {
            float systemCPU;
            double? cpu = null;

            if (TryGetSystemCPU(out systemCPU))
            {
                cpu = Math.Round(systemCPU, 2);
            }

            return new CPUStats
            {
                TimeStamp = DateTimeOffset.UtcNow,
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
    }
}
