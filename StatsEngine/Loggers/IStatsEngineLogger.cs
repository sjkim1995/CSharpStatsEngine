using System;
using StatsEngine.Persistence;

namespace StatsEngine.Logging
{
    interface IStatsEngineLogger : IDisposable
    {
        /// <summary>
        /// Start collecting statistics and writing it to a buffer
        /// </summary>
        void StartLogging();

    }
}
