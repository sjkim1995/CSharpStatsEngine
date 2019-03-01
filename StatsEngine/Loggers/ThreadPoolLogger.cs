﻿using System;
using System.Threading;
using StatsEngine.Persistence;
using StatsEngine.Shared.Types;

namespace StatsEngine.Logging
{

    class ThreadPoolLogger : StatsEngineLogger
    {
        public ThreadPoolLogger(TimeSpan logFrequency, StatsBuffer<IMachineStat> buf) : base(logFrequency, buf)
        {
        }

        /// <summary>
        /// Returns the current thread pool usage statistics for the CURRENT AppDomain/Process
        /// </summary>
        public override IMachineStat GetStat()
        {
            //BusyThreads =  TP.GetMaxThreads() –TP.GetAVailable();
            //If BusyThreads >= TP.GetMinThreads(), then threadpool growth throttling is possible.

            int maxIoThreads, maxWorkerThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIoThreads);

            int freeIoThreads, freeWorkerThreads;
            ThreadPool.GetAvailableThreads(out freeWorkerThreads, out freeIoThreads);

            int minIoThreads, minWorkerThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIoThreads);

            int busyIoThreads = maxIoThreads - freeIoThreads;
            int busyWorkerThreads = maxWorkerThreads - freeWorkerThreads;

            return new ThreadPoolStats
            {
                TimeStamp = DateTimeOffset.UtcNow,
                BusyIoThreads = busyIoThreads,
                MinIoThreads = minIoThreads,
                MaxIoThreads = maxIoThreads,
                BusyWorkerThreads = busyWorkerThreads,
                MinWorkerThreads = minWorkerThreads,
                MaxWorkerThreads = maxWorkerThreads,
            };
        }
    }
}