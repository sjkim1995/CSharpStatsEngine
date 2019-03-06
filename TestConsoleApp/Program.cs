using System;
using StatsEngine;

namespace StatsEngine.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to exit the program...\n");

            var statsEngine = new StatsEngine();
            statsEngine.StartLogging();

            Console.ReadKey();
        }
    }
}
