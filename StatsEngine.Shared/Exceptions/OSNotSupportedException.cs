using System;
using System.Runtime.InteropServices;


namespace StatsEngine.Shared.Exceptions
{
    class OSNotSupportedException : Exception
    {
        public OSNotSupportedException() {
            Console.WriteLine("We only support Windows/Linux/OSX");
        }

        public OSNotSupportedException(OSPlatform platform, StatType statType)
        {
            Console.WriteLine($"We currently do not support logging for {statType.ToString()} on {platform.ToString()}");
        }

    }
}
