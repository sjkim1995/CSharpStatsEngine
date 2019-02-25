using System;
using CircularBuffer;

namespace StatsEngine.Loggers
{
    interface IStatsEngineLogger<T> : IDisposable
    {
        /// <summary>
        /// Calls GetStat and passes its result to LogStat on every _logFrequency
        /// </summary>
        void StartLogging(CircularBuffer<T> buf);

        /// <summary>
        /// Writes the given stat struct to the front of the circular buffer
        /// </summary>
        void LogStat(CircularBuffer<T> buf, T stat);

        /// <summary>
        /// Returns the stat
        /// </summary>
        T GetStat();

    }
}
