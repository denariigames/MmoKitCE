using System;
using System.Collections.Generic;
using MultiplayerARPG.Server.Time;

namespace MultiplayerARPG.Server.Scheduling
{
    public sealed class ServerDelayedTaskSystem : ITickSystem
    {
        public string Name => "DelayedTasks";

        // ---- Min-heap ordered by ExecuteAt ----
        private readonly List<DelayedTask> heap = new List<DelayedTask>(64);

        // Pool to avoid allocations
        private readonly Stack<DelayedTask> pool = new Stack<DelayedTask>(64);

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            // Use authoritative server time (not ctx.Now) so tasks fire based on the real clock.
            double now = ServerClock.Time;

            while (heap.Count > 0)
            {
                DelayedTask task = heap[0];
                if (task.ExecuteAt > now)
                    break;

                Pop();

                if (!task.Cancelled && task.Callback != null)
                {
                    try
                    {
                        task.Callback();
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        UnityEngine.Debug.LogError($"[ServerDelayedTask] Exception:\n{ex}");
#endif
                    }
                }

                Recycle(task);
            }
        }

        public void Commit(in TickContext ctx) { }

        internal IScheduledTaskHandle Schedule(double delaySeconds, Action callback)
        {
            DelayedTask task = pool.Count > 0 ? pool.Pop() : new DelayedTask();
            task.ExecuteAt = ServerClock.Time + delaySeconds;
            task.Callback = callback;
            task.Cancelled = false;

            Push(task);
            return task;
        }

        private void Push(DelayedTask task)
        {
            heap.Add(task);
            int i = heap.Count - 1;

            while (i > 0)
            {
                int parent = (i - 1) >> 1;
                if (heap[parent].ExecuteAt <= task.ExecuteAt)
                    break;

                heap[i] = heap[parent];
                i = parent;
            }

            heap[i] = task;
        }

        private void Pop()
        {
            int last = heap.Count - 1;
            DelayedTask root = heap[0];
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
                int smallest = (right < heap.Count && heap[right].ExecuteAt < heap[left].ExecuteAt) ? right : left;

                if (heap[smallest].ExecuteAt >= tail.ExecuteAt)
                    break;

                heap[i] = heap[smallest];
                i = smallest;
            }

            heap[i] = tail;
        }

        private void Recycle(DelayedTask task)
        {
            task.Reset();
            pool.Push(task);
        }

        private sealed class DelayedTask : IScheduledTaskHandle
        {
            public double ExecuteAt;
            public Action Callback;
            public bool Cancelled;

            public bool IsActive => !Cancelled && Callback != null;

            public void Cancel()
            {
                Cancelled = true;
                Callback = null;
            }

            public void Reset()
            {
                ExecuteAt = 0;
                Callback = null;
                Cancelled = false;
            }
        }
    }

    public interface IScheduledTaskHandle
    {
        bool IsActive { get; }
        void Cancel();
    }
}
