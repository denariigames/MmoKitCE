using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(int.MaxValue)]
    public class JobHandleCompleter : MonoBehaviour
    {
        private static JobHandleCompleter s_singleton;
        private static bool s_isQuitting;

        public static JobHandleCompleter Singleton
        {
            get
            {
                if (s_singleton == null && !s_isQuitting)
                {
                    s_singleton = new GameObject("_JobHandleCompleter_Instance").AddComponent<JobHandleCompleter>();
                    DontDestroyOnLoad(s_singleton.gameObject);
                }
                return s_singleton;
            }
        }

        private readonly static List<JobHandle> s_jobHandles = new List<JobHandle>(128);

        // NOTE: Static events can hold managed shells alive if subscribers do not unsubscribe.
        // Keep the field for compatibility, but clear it during teardown and invoke defensively.
        public static System.Action OnCompletedEvent = null;

        public static int DebugPendingJobCount
        {
            get { return s_jobHandles.Count; }
        }

        public void AddJobHandle(JobHandle jobHandle)
        {
            s_jobHandles.Add(jobHandle);
        }

        private void LateUpdate()
        {
            CompletePendingJobsNoAlloc();
            InvokeCompletedEventSafely();
        }

        private static void CompletePendingJobsNoAlloc()
        {
            if (s_jobHandles.Count == 0)
                return;

            try
            {
                // Avoid List.ToArray() + NativeArray<JobHandle>(..., Allocator.TempJob) every frame.
                // Completing handles individually is allocation-free and avoids native TempJob churn.
                for (int i = 0; i < s_jobHandles.Count; ++i)
                    s_jobHandles[i].Complete();
            }
            finally
            {
                // Always clear, even if a job completion throws, so stale handles are not retained forever.
                s_jobHandles.Clear();
            }
        }

        private static void InvokeCompletedEventSafely()
        {
            System.Action completedEvent = OnCompletedEvent;
            if (completedEvent == null)
                return;

            System.Delegate[] invocationList = completedEvent.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                try
                {
                    ((System.Action)invocationList[i]).Invoke();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private void OnDestroy()
        {
            if (s_singleton == this)
                s_singleton = null;

            s_jobHandles.Clear();
            OnCompletedEvent = null;
        }

        private void OnApplicationQuit()
        {
            s_isQuitting = true;
            s_jobHandles.Clear();
            OnCompletedEvent = null;
        }
    }
}
