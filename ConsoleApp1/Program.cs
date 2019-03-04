using System;
using StatsEngine.Persistence;

namespace StatsEngine.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to exit the program...\n");

            var bufferManager = new BufferManager();
            var loggingManager = new LoggingManager(bufferManager);

            loggingManager.StartLogging();
            Console.ReadKey();
        }
    }
}
