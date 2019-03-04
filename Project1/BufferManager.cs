using System;
using System.Collections.Generic;
using StatsEngine.Shared;

namespace StatsEngine.Persistence
{
    public class BufferManager
    {
        private readonly int capacity;
        private Dictionary<StatType, StatsBuffer<IMachineStat>> bufferMap;

        public BufferManager(int capacity)
        {
            this.capacity = capacity;

            InitBufferMap();
        }

        public BufferManager() : this(SEConstants.DefaultBufferSize)
        {
        }

        private void InitBufferMap()
        {
            bufferMap = new Dictionary<StatType, StatsBuffer<IMachineStat>>();
            
            // Generate a buffer for each type of statistic we're collecting
            foreach (StatType type in Enum.GetValues(typeof(StatType))) 
            {
                bufferMap.Add(type, new StatsBuffer<IMachineStat>(type, capacity));
            }
        }

        public StatsBuffer<IMachineStat> GetBuffer(StatType type)
        {
            return bufferMap[type];
        }
    }
}
