using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StatsEngine.Persistence;
using StatsEngine.Shared;

namespace StatsEngine.Logging
{
    internal abstract class StatsEngineLogger : IDisposable
    {
        protected TimeSpan _logFrequency;
        private bool _disposed;
        protected StatsBuffer<IMachineStat> _buf;

        public StatsEngineLogger(TimeSpan logFrequency, StatsBuffer<IMachineStat> buf)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            _buf = buf;
        }

        /// <summary>
        /// Calls GetStat() and passes the result to LogStat on the TimeInterval specified by logFrequency
        /// </summary>
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

        /// <summary>
        /// Writes an IMachineStat to the front of a circular buffer in persistence layer
        /// </summary>
        public void LogStat(IMachineStat stat)
        {
            _buf.AddStat(stat);
        }

        /// <summary>
        /// Returns an IMachineStat about the system (threadpool, bandwidth, CPU, etc.)
        /// </summary>
        public abstract IMachineStat GetStat();

        /// <summary>
        /// Stops logging when called
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }
    }
}
