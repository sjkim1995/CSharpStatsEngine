using System;
using StatsEngine.Persistence;
using StatsEngine.Shared; 

namespace StatsEngine.Analysis
{
    public class AnalysisManager
    {
        private BufferManager _bufMgr;

        public AnalysisManager(BufferManager bufMgr)
        {
            _bufMgr = bufMgr;
        }

        public StatsBuffer<MachineStat> GetStatBuffer(StatType statType)
        {
            return _bufMgr.GetBuffer(statType);
        }

        public MachineStat GetLatestStat(StatType statType)
        {
            var buf = GetStatBuffer(statType);
            MachineStat latestStat = buf.PeekMostRecentStat();

            return latestStat;
        }

    }
}
