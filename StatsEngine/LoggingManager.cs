using System;
using System.Collections.Generic;
using StatsEngine.Logging;
using StatsEngine.Persistence;
using StatsEngine.Shared.Types;

namespace StatsEngine
{
    public class LoggingManager
    {
        // Set of loggers/perf counters
        private HashSet<StatsEngineLogger> loggerSet;
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
            InitLoggerSet();
        }

        private void InitLoggerSet()
        {
            loggerSet = new HashSet<StatsEngineLogger>
            { 
                // Add new loggers here...
                new BandwidthLogger(_logFrequency, bufMgr.GetBuffer(StatType.Bandwidth)),
                new CPULogger(_logFrequency, bufMgr.GetBuffer(StatType.CPU)),
                new ThreadPoolLogger(_logFrequency, bufMgr.GetBuffer(StatType.ThreadPool)),
                new PageFaultLogger(_logFrequency, bufMgr.GetBuffer(StatType.PageFaults))
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
