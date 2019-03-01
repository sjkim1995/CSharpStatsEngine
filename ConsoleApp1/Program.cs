using System;
using StatsEngine.Persistence;
using StatsEngine.Logging;

namespace StatsEngine.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to exit the program...\n");

            var bufferManager = new BufferManager(30);
            var loggingManager = new LoggingManager(TimeSpan.FromSeconds(3), bufferManager);

            loggingManager.StartLoggers();
            Console.ReadKey();
        }
    }
}
