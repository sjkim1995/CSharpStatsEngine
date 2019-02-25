using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using CircularBuffer;

namespace StatsEngine.Loggers
{
    public struct ThreadPoolStats
    {
        public DateTime TimeStamp { get; set; }

        public int BusyIoThreads { get; set; }

        public int MinIoThreads { get; set; }

        public int MaxIoThreads { get; set; }

        public int BusyWorkerThreads { get; set; }

        public int MinWorkerThreads { get; set; }

        public int MaxWorkerThreads { get; set; }
    }

    class ThreadPoolLogger : IStatsEngineLogger<ThreadPoolStats>
    {
        private TimeSpan _logFrequency;
        private bool _disposed;
        private bool _writeToConsole;

        public ThreadPoolLogger(TimeSpan logFrequency, bool writeToConsole = false)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            _writeToConsole = writeToConsole;
        }

        public async void StartLogging(CircularBuffer<ThreadPoolStats> buf)
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

        public virtual void LogStat(CircularBuffer<ThreadPoolStats> buf, ThreadPoolStats stat)
        {
            buf.PushFront(stat);

            if (_writeToConsole)
            {
                string message = string.Format("[{0}] IOCP:(Busy={1},Min={2},Max={3}), WORKER:(Busy={4},Min={5},Max={6})",
                                DateTimeOffset.UtcNow.ToString("u"),
                                stat.BusyIoThreads, stat.MinIoThreads, stat.MaxIoThreads,
                                stat.BusyWorkerThreads, stat.MinWorkerThreads, stat.MaxWorkerThreads
                                );

                Trace.WriteLine(message);
            }
            
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