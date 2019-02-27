using StatsEngine.Types;
using System.Collections.Generic;

namespace StatsEngine.Persistence
{
    public class BufferManager
    {
        private readonly int capacity;
        public Dictionary<StatType, dynamic> bufferMap;

        public BufferManager(int capacity)
        {
            this.capacity = capacity;

            CreateBufferMap();
        }

        private void CreateBufferMap()
        {
            bufferMap = new Dictionary<StatType, dynamic>
            {
                // Add new buffers here...
                { StatType.Bandwidth, new StatsBuffer<BandwidthStats>(capacity) },
                { StatType.SystemCPU, new StatsBuffer<SystemCPUStats>(capacity) },
                { StatType.ThreadPool, new StatsBuffer<ThreadPoolStats>(capacity) }
            };
        }

    }
}
