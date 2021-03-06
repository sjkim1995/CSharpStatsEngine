﻿using System;
using StatsEngine.Shared;

namespace StatsEngine.Persistence
{
    public class StatsBuffer<T> where T : MachineStat
    {
        CircularBuffer<T> buffer;
        readonly StatType _statType; 

        public StatsBuffer(StatType statType, int capacity)
        {
            buffer = new CircularBuffer<T>(capacity);
            _statType = statType;
        }

        /// <summary>
        /// Number of elements in the buffer
        /// </summary>
        public int Size { get { return buffer.Size; } }

        /// <summary>
        /// Maximum capacity of the stats buffer (as set when instantiating the buffer)
        /// </summary>
        public int Capacity { get { return buffer.Capacity; } }

        /// <summary>
        /// Returns true if Size(buffer) == Capacity
        /// </summary>
        public bool IsFull { get { return buffer.IsFull; } }

        /// <summary>
        /// Returns true if Size(buffer) == 0
        /// </summary>
        public bool IsEmpty { get { return buffer.IsEmpty; } }

        /// <summary>
        /// Adds a stat to the front of the buffer
        /// </summary>
        public void PushStat(T item)
        {
            buffer.PushFront(item);

            // for testing purposes
            Console.WriteLine(item.ToLogString());
        }

        /// <summary>
        /// Removes the most recently added stat
        /// </summary>
        public void PopMostRecentStat()
        {
            buffer.PopFront();
        }

        /// <summary>
        /// Removes the least recently added stat
        /// </summary>
        public void PopLeastRecentStat()
        {
            buffer.PopBack();
        }

        /// <summary>
        /// Returns the most recently collected stat
        /// </summary>
        public T PeekMostRecentStat()
        {
            return buffer.Front();
        }

        /// <summary>
        /// Returns the least recently collected stat
        /// </summary>
        public T PeekLeastRecentStat()
        {
            return buffer.Back();
        }

        /// <summary>
        /// Returns the stat at the given index
        /// </summary>
        public T PeekStatAtIndex(int index)
        {
            return buffer._buffer[index];
        }

        /// <summary>
        /// Returns the stats as an array
        /// </summary>
        public T[] ToArray()
        {
            return buffer.ToArray();
        }
    }
}
