using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StatsEngine.Logging
{
    abstract class GenericPerSecondLogger : InitHolder
    {

        virtual protected void OnItemPSValue(int index, long value) { }
        virtual protected void OnCreate() { }
        abstract protected void GetData(out long d1, out long d2, out long d3, out long d4, out long d5, out long d6);

        virtual protected void AppendOther(StringBuilder sb)
        {

        }

        long[] last = new long[6];
        DateTime timePrev;
        string name;
        string[] labels;
        int interval;
        bool includeCurrent;
        public GenericPerSecondLogger(string name, string[] labels, int interval = 1, bool includeCurrent = false)
        {
            DefineCreate(() => {
                OnCreate();
                timePrev = DateTime.UtcNow;
                GetData(out last[0], out last[1], out last[2], out last[3], out last[4], out last[5]);
            });
            this.labels = labels.ToArray();
            this.name = name;
            this.interval = interval;
            this.includeCurrent = includeCurrent;
        }

        virtual public Task<int> Log()
        {
            EnsureInit();
            DateTime now = DateTime.UtcNow;
            var elapsed = now - timePrev;
            timePrev = now;
            long[] d = new long[6];
            GetData(out d[0], out d[1], out d[2], out d[3], out d[4], out d[5]);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}]", name);
            for (int i = 0; i < labels.Length; i++)
            {
                long value = (long)((d[i] - last[i]) * interval / elapsed.TotalSeconds);
                OnItemPSValue(i, value);
                sb.AppendFormat(" [{0}:{1}]", labels[i], value);
                last[i] = d[i];
            }
            sb.AppendFormat(" per {0} sec in last {1} secs", interval, (int)elapsed.TotalSeconds);
            AppendOther(sb);
            if (includeCurrent)
            {
                sb = new StringBuilder();
                sb.AppendFormat("[{0} Current]", name);
                for (int i = 0; i < labels.Length; i++)
                {
                    sb.AppendFormat(" [{0}:{1}]", labels[i], d[i]);
                }

            }
            return Task.FromResult(0);
        }
    }

    abstract public class InitHolder
    {
        Action initAction;
        volatile bool inited;
        object lo = new object();

        protected void DefineCreate(Action initAction)
        {
            this.initAction = initAction;
        }

        protected bool EnsureInit()
        {
            if (inited) return true;
            lock (lo)
            {
                initAction();
                inited = true;
                return true;
            }
        }

    }
}
