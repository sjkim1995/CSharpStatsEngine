using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngine.Logging
{
    class PageFaultHelper : GenericPerSecondLogger
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("ntdll.dll")]
        public static extern uint NtQuerySystemInformation(uint kind, IntPtr returnStruct, uint length, out uint returnLength);

        static IntPtr Memory;
        static uint Size;

        public static uint LastPageFaultsPerSecond { get; protected set; }

        public static uint PagesFaulted { get; protected set; }

        public static uint PageReadsFaulted { get; protected set; }

        protected override void OnItemPSValue(int index, long value)
        {
            if (index == 0)
            {
                LastPageFaultsPerSecond = (uint)value;
            }
        }

        public PageFaultHelper(TimeSpan intervalInSeconds) : base("Page faults per sec", new string[] { "faults", "reads" }, Convert.ToInt32(intervalInSeconds.TotalSeconds))
        {
        }

        protected override void GetData(out long d1, out long d2, out long d3, out long d4, out long d5, out long d6)
        {
            uint pf, pfr;
            GetNumbers(out pf, out pfr);

            PagesFaulted = pf;
            PageReadsFaulted = pfr;

            d1 = pf;
            d2 = pfr;
            d3 = d4 = d5 = d6 = 0;
        }

        void GetNumbers(out uint pagesFaulted, out uint pageReadsFaulted)
        {
            if (Memory == IntPtr.Zero)
            {
                uint rval = NtQuerySystemInformation(2, IntPtr.Zero, 0, out Size);
                if (rval == 0xc0000004 && Size != 0)
                {
                    Memory = Marshal.AllocHGlobal((int)Size);
                }
                else
                {
                    throw new Exception("Unable to initialize page fault info.");
                }
            }
            uint r = NtQuerySystemInformation(2, Memory, Size, out Size);
            if (r != 0)
            {
                throw new Exception("Unable to obtain page fault info.");
            }
            pagesFaulted = (uint)Marshal.ReadInt32(Memory + 80);
            pageReadsFaulted = (uint)Marshal.ReadInt32(Memory + 84);
        }
    }
}
