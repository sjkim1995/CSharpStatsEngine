using System;
using StatsEngine.Persistence;
using StatsEngine.Logging;
using StatsEngine.Analysis;

namespace StatsEngine
{
    public class StatsEngine
    {
        private BufferManager bufMgr;
        private LoggingManager loggingMgr;
        private AnalysisManager analysisMgr;

        public StatsEngine()
        {
            bufMgr = new BufferManager();
            loggingMgr = new LoggingManager(bufMgr);
            analysisMgr = new AnalysisManager(bufMgr);
        }

        public void StartLogging()
        {
            loggingMgr.StartLogging();
        }

        public void StopLogging()
        {
            loggingMgr.StopLogging();
        }


    }
}
