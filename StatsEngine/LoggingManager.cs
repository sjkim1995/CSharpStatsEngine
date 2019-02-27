using StatsEngine.Logging;
using StatsEngine.Persistence;
using System;
using System.Collections.Generic;
using StatsEngine.Types;

namespace StatsEngine
{
    public class LoggingManager
    {
        // Set of loggers/perf counters
        private HashSet<IStatsEngineLogger> loggerSet;
        private TimeSpan _logFrequency;

        private BufferManager bufMgr;

        public LoggingManager(TimeSpan logFrequency, BufferManager bufferManager)
        {
            if (logFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("logFrequency");
            }

            _logFrequency = logFrequency;
            bufMgr = bufferManager;

            // Instantiate loggerSet and add loggers
            CreateLoggerSet();
        }

        private void CreateLoggerSet()
        {
            loggerSet = new HashSet<IStatsEngineLogger>
            { 
                // Add new loggers here...
                new BandwidthLogger(_logFrequency, bufMgr.bufferMap[Types.StatType.Bandwidth]),
                new SystemCPULogger(_logFrequency, bufMgr.bufferMap[Types.StatType.SystemCPU]),
                new ThreadPoolLogger(_logFrequency, bufMgr.bufferMap[Types.StatType.ThreadPool])
            };
        }

        public void StartLoggers()
        {
            foreach (var logger in loggerSet)
            {
                logger.StartLogging();
            }
        }

    }
}
