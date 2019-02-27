using System;

namespace StatsEngine.Types
{
    public struct ThreadPoolStats
    {
        public DateTimeOffset TimeStamp { get; set; }

        public int BusyIoThreads { get; set; }

        public int MinIoThreads { get; set; }

        public int MaxIoThreads { get; set; }

        public int BusyWorkerThreads { get; set; }

        public int MinWorkerThreads { get; set; }

        public int MaxWorkerThreads { get; set; }

        public string FormatToString()
        {
            return string.Format("[{0}] IOCP:(Busy={1},Min={2},Max={3}), WORKER:(Busy={4},Min={5},Max={6})",
                            TimeStamp.ToString("u"),
                            BusyIoThreads, MinIoThreads, MaxIoThreads,
                            BusyWorkerThreads, MinWorkerThreads, MaxWorkerThreads);
        }
    }
}
