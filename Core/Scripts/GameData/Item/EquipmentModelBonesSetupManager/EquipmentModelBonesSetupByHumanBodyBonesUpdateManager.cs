using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(int.MaxValue)]
    public class EquipmentModelBonesSetupByHumanBodyBonesUpdateManager : MonoBehaviour
    {
        private static EquipmentModelBonesSetupByHumanBodyBonesUpdateManager _instance;
        public static EquipmentModelBonesSetupByHumanBodyBonesUpdateManager Instance => _instance != null ? _instance : (_instance = CreateInstance());

        private static EquipmentModelBonesSetupByHumanBodyBonesUpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(EquipmentModelBonesSetupByHumanBodyBonesUpdateManager))
            {
                hideFlags = HideFlags.DontSave,
            };
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            else
#endif
            {
                DontDestroyOnLoad(gameObject);
            }
            return gameObject.AddComponent<EquipmentModelBonesSetupByHumanBodyBonesUpdateManager>();
        }

        private readonly List<Transform> _allSrc = new List<Transform>(1024);
        private readonly List<Transform> _allDst = new List<Transform>(1024);
        private NativeList<TransformData> _srcArray;
        private TransformAccessArray _dstArray;
        private JobHandle _jobHandle;

        private void Awake()
        {
            _srcArray = new NativeList<TransformData>(1024, Allocator.Persistent);
            _dstArray = new TransformAccessArray(1024);
        }

        private void OnDestroy()
        {
            _jobHandle.Complete();

            if (_srcArray.IsCreated)
                _srcArray.Dispose();

            if (_dstArray.isCreated)
                _dstArray.Dispose();

            _allSrc.Clear();
            _allDst.Clear();
        }

        public void Register(List<Transform> src, List<Transform> dst)
        {
            _allSrc.AddRange(src);
            _allDst.AddRange(dst);
        }

        private void LateUpdate()
        {
            // Ensure previous job is done
            _jobHandle.Complete();

            // Prepare empty array
            int length = _allSrc.Count;
            _srcArray.Clear();

            int maxLength = 0;
            // Push to src list
            for (int i = length - 1; i >= 0; --i)
            {
                Transform srcTransform = _allSrc[i];
                Transform dstTransform = _allDst[i];
                if (srcTransform == null || dstTransform == null)
                {
                    continue;
                }
                _srcArray.Add(new TransformData()
                {
                    position = srcTransform.position,
                    rotation = srcTransform.rotation,
                    localScale = srcTransform.localScale,
                });
                if (maxLength < _dstArray.length)
                    _dstArray[maxLength] = dstTransform;
                else
                    _dstArray.Add(dstTransform);
                ++maxLength;
            }

            // Trim stale tail entries so the job only iterates what's needed
            while (_dstArray.length > maxLength)
            {
                _dstArray.RemoveAtSwapBack(_dstArray.length - 1);
            }

            _allSrc.Clear();
            _allDst.Clear();

            // Schedule ONE big job
            CopyTransformsJob job = new CopyTransformsJob
            {
                sourceTransforms = _srcArray,
            };

            _jobHandle = job.Schedule(_dstArray);
        }

        [BurstCompile]
        private struct CopyTransformsJob : IJobParallelForTransform
        {
            [ReadOnly]
            public NativeList<TransformData> sourceTransforms;

            public void Execute(int index, TransformAccess destination)
            {
                if (!destination.isValid)
                    return;

                TransformData source = sourceTransforms[index];

                destination.position = source.position;
                destination.rotation = source.rotation;
                destination.localScale = source.localScale;
            }
        }

        [BurstCompile]
        private struct TransformData
        {
            public float3 position;
            public quaternion rotation;
            public float3 localScale;
        }
    }
}