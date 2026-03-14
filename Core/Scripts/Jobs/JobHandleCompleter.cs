using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(int.MaxValue)]
    public class JobHandleCompleter : MonoBehaviour
    {
        private static JobHandleCompleter s_singleton;
        public static JobHandleCompleter Singleton
        {
            get
            {
                if (s_singleton == null)
                {
                    s_singleton = new GameObject("_JobHandleCompleter_Instance").AddComponent<JobHandleCompleter>();
                    DontDestroyOnLoad(s_singleton.gameObject);
                }
                return s_singleton;
            }
        }

        private readonly static List<JobHandle> s_jobHandles = new List<JobHandle>();
        public static System.Action OnCompletedEvent = null;

        public void AddJobHandle(JobHandle jobHandle)
        {
            s_jobHandles.Add(jobHandle);
        }

        private void LateUpdate()
        {
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(s_jobHandles.ToArray(), Allocator.TempJob);
            s_jobHandles.Clear();
            JobHandle.CompleteAll(jobHandles);
            OnCompletedEvent?.Invoke();
            jobHandles.Dispose();
        }
    }
}
