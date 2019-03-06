using System;
using System.Runtime.InteropServices;
using StatsEngine.Shared.Exceptions;

namespace StatsEngine.Shared
{
    // Base class for all stat types collected about the client machine

    public abstract class MachineStat
    {

        public MachineStat()
        {
            SetOSPlatform();
        }

        public DateTimeOffset TimeStamp { get; set; }

        OSPlatform _OSPlatform { get; set; }

        public abstract string ToLogString();

        public void SetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _OSPlatform = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _OSPlatform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _OSPlatform = OSPlatform.OSX;
            }
            else
            {
                throw new OSNotSupportedException();
            }
        }

    }
}
