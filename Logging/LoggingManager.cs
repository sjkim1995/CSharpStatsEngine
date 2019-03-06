using System;
using System.Collections.Generic;
using StatsEngine.Logging;
using StatsEngine.Persistence;
using StatsEngine.Shared;

namespace StatsEngine.Logging
{
    public class LoggingManager
    {
        // Set of loggers/perf counters
        private HashSet<MachineStatLogger> loggerSet;
        private TimeSpan _logFrequency;

        private BufferManager bufMgr;

        public LoggingManager(BufferManager bufferManager, TimeSpan logFrequency)
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

        public LoggingManager(BufferManager bufferManager) : this(bufferManager, TimeSpan.FromSeconds(SEConstants.DefaultLogInterval))
        {
        }

        private void InitLoggerSet()
        {
            loggerSet = new HashSet<MachineStatLogger>
            { 
                // Add new loggers here...
                //new BandwidthLogger(_logFrequency, bufMgr.GetBuffer(StatType.Bandwidth)),
                //new CPULogger(_logFrequency, bufMgr.GetBuffer(StatType.CPU)),
                //new ThreadPoolLogger(_logFrequency, bufMgr.GetBuffer(StatType.ThreadPool)),
                new PageFaultLogger(_logFrequency, bufMgr.GetBuffer(StatType.PageFaults))
            };
        }

        public void StartLogging()
        {
            foreach (var logger in loggerSet)
            {
                logger.StartLogging();
            }
        }

        public void StopLogging()
        {
            foreach(var logger in loggerSet)
            {
                logger.Dispose();
            }
        }

    }
}
