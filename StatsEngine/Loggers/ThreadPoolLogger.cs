using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using StatsEngine.Persistence;
using StatsEngine.Types;

namespace StatsEngine.Logging
{

    class ThreadPoolLogger : IStatsEngineLogger
    {
        private TimeSpan _logFrequency;
        private bool _disposed;
        private StatsBuffer<ThreadPoolStats> _buf;

        public ThreadPoolLogger(TimeSpan logFrequency, StatsBuffer<ThreadPoolStats> buf)
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

        public virtual void LogStat(ThreadPoolStats stat)
        {
            _buf.AddStat(stat);         
        }

        /// <summary>
        /// Returns the current thread pool usage statistics for the CURRENT AppDomain/Process
        /// </summary>
        public ThreadPoolStats GetStat()
        {
            //BusyThreads =  TP.GetMaxThreads() –TP.GetAVailable();
            //If BusyThreads >= TP.GetMinThreads(), then threadpool growth throttling is possible.

            int maxIoThreads, maxWorkerThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIoThreads);

            int freeIoThreads, freeWorkerThreads;
            ThreadPool.GetAvailableThreads(out freeWorkerThreads, out freeIoThreads);

            int minIoThreads, minWorkerThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIoThreads);

            int busyIoThreads = maxIoThreads - freeIoThreads;
            int busyWorkerThreads = maxWorkerThreads - freeWorkerThreads;

            return new ThreadPoolStats
            {
                TimeStamp = DateTime.UtcNow,
                BusyIoThreads = busyIoThreads,
                MinIoThreads = minIoThreads,
                MaxIoThreads = maxIoThreads,
                BusyWorkerThreads = busyWorkerThreads,
                MinWorkerThreads = minWorkerThreads,
                MaxWorkerThreads = maxWorkerThreads,
            };
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}