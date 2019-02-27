using System;
using StatsEngine.Persistence;
using StatsEngine.Logging;

namespace StatsEngine.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferManager = new BufferManager(10);
            var loggingManager = new LoggingManager(TimeSpan.FromSeconds(3), bufferManager);



            loggingManager.StartLoggers();

        }
    }
}
