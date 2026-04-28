//updated
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MultiplayerARPG.Server.Time;

namespace MultiplayerARPG.Server.Scheduling
{
    public interface IScheduledTaskHandle
    {
        bool IsActive { get; }
        void Cancel();
    }

    public sealed class DelayedTaskMetrics
    {
        public int PendingCount;
        public int PoolCount;
        public long ScheduledCount;
        public long ExecutedCount;
        public long CancelledCount;
        public long ExceptionCount;
        public long BudgetStopCount;
        public long MaxCallbackStopCount;
        public double LastTickMs;
        public double MaxTickMs;
        public int LastExecutedCallbacks;
    }

    public sealed class ServerDelayedTaskSystem : ITickSystem
    {
        public string Name { get { return "DelayedTasks"; } }

        public int MaxCallbacksPerTick = 256;
        public double MaxMsPerTick = 1.0d;

        private readonly List<DelayedTask> heap = new List<DelayedTask>(128);
        private readonly Stack<DelayedTask> pool = new Stack<DelayedTask>(128);
        private readonly DelayedTaskMetrics metrics = new DelayedTaskMetrics();
        private readonly Stopwatch tickWatch = new Stopwatch();
        private long nextSequence;

        public DelayedTaskMetrics Metrics { get { return metrics; } }
        public int PendingCount { get { return heap.Count; } }

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            double now = ServerClock.Time;
            int executed = 0;
            tickWatch.Restart();

            while (heap.Count > 0)
            {
                if (MaxCallbacksPerTick > 0 && executed >= MaxCallbacksPerTick)
                {
                    metrics.MaxCallbackStopCount++;
                    break;
                }

                if (MaxMsPerTick > 0d && tickWatch.Elapsed.TotalMilliseconds >= MaxMsPerTick)
                {
                    metrics.BudgetStopCount++;
                    break;
                }

                DelayedTask task = heap[0];
                if (task.ExecuteAt > now)
                    break;

                PopRoot();

                if (task.Cancelled || task.Callback == null)
                {
                    Recycle(task);
                    continue;
                }

                Action callback = task.Callback;
                task.Callback = null;
                task.Cancelled = true;
                task.InHeap = false;

                try
                {
                    callback();
                    metrics.ExecutedCount++;
                }
                catch (Exception ex)
                {
                    metrics.ExceptionCount++;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    UnityEngine.Debug.LogError("[ServerDelayedTask] Exception:\n" + ex);
#else
                    UnityEngine.Debug.LogError("[ServerDelayedTask] Exception.");
#endif
                }

                executed++;
                Recycle(task);
            }

            tickWatch.Stop();
            metrics.LastTickMs = tickWatch.Elapsed.TotalMilliseconds;
            if (metrics.LastTickMs > metrics.MaxTickMs)
                metrics.MaxTickMs = metrics.LastTickMs;
            metrics.LastExecutedCallbacks = executed;
            metrics.PendingCount = heap.Count;
            metrics.PoolCount = pool.Count;
        }

        public void Commit(in TickContext ctx) { }

        public void Clear()
        {
            for (int i = 0; i < heap.Count; i++)
            {
                DelayedTask task = heap[i];
                task.Cancelled = true;
                task.Callback = null;
                task.InHeap = false;
                task.Generation++;
                pool.Push(task);
            }
            heap.Clear();
            metrics.PendingCount = 0;
            metrics.PoolCount = pool.Count;
        }

        internal IScheduledTaskHandle Schedule(double delaySeconds, Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (delaySeconds < 0d)
                delaySeconds = 0d;

            DelayedTask task = pool.Count > 0 ? pool.Pop() : new DelayedTask();
            task.Generation++;
            task.ExecuteAt = ServerClock.Time + delaySeconds;
            task.Sequence = nextSequence++;
            task.Callback = callback;
            task.Cancelled = false;
            task.InHeap = true;

            Push(task);
            metrics.ScheduledCount++;
            metrics.PendingCount = heap.Count;
            metrics.PoolCount = pool.Count;
            return new ScheduledTaskHandle(this, task, task.Generation);
        }

        private bool TryCancel(DelayedTask task, int generation)
        {
            if (task == null || task.Generation != generation || !task.InHeap || task.Cancelled)
                return false;

            task.Cancelled = true;
            task.Callback = null;
            metrics.CancelledCount++;
            return true;
        }

        private bool IsActive(DelayedTask task, int generation)
        {
            return task != null && task.Generation == generation && task.InHeap && !task.Cancelled && task.Callback != null;
        }

        private void Push(DelayedTask task)
        {
            heap.Add(task);
            int i = heap.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) >> 1;
                if (Compare(heap[parent], task) <= 0)
                    break;
                heap[i] = heap[parent];
                i = parent;
            }
            heap[i] = task;
        }

        private void PopRoot()
        {
            int last = heap.Count - 1;
            DelayedTask tail = heap[last];
            heap.RemoveAt(last);
            if (last == 0)
                return;

            int i = 0;
            while (true)
            {
                int left = (i << 1) + 1;
                if (left >= heap.Count)
                    break;

                int right = left + 1;
                int smallest = right < heap.Count && Compare(heap[right], heap[left]) < 0 ? right : left;
                if (Compare(heap[smallest], tail) >= 0)
                    break;

                heap[i] = heap[smallest];
                i = smallest;
            }
            heap[i] = tail;
        }

        private static int Compare(DelayedTask a, DelayedTask b)
        {
            int timeCompare = a.ExecuteAt.CompareTo(b.ExecuteAt);
            if (timeCompare != 0)
                return timeCompare;
            return a.Sequence.CompareTo(b.Sequence);
        }

        private void Recycle(DelayedTask task)
        {
            task.ExecuteAt = 0d;
            task.Sequence = 0L;
            task.Callback = null;
            task.Cancelled = false;
            task.InHeap = false;
            task.Generation++;
            pool.Push(task);
        }

        private sealed class ScheduledTaskHandle : IScheduledTaskHandle
        {
            private readonly ServerDelayedTaskSystem owner;
            private readonly DelayedTask task;
            private readonly int generation;

            public ScheduledTaskHandle(ServerDelayedTaskSystem owner, DelayedTask task, int generation)
            {
                this.owner = owner;
                this.task = task;
                this.generation = generation;
            }

            public bool IsActive
            {
                get { return owner != null && owner.IsActive(task, generation); }
            }

            public void Cancel()
            {
                if (owner != null)
                    owner.TryCancel(task, generation);
            }
        }

        private sealed class DelayedTask
        {
            public double ExecuteAt;
            public long Sequence;
            public Action Callback;
            public bool Cancelled;
            public bool InHeap;
            public int Generation;
        }
    }
}
