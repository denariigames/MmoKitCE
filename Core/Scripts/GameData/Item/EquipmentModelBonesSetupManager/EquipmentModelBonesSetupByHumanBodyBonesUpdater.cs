using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(int.MaxValue - 1)]
    public class EquipmentModelBonesSetupByHumanBodyBonesUpdater : MonoBehaviour
    {
        [System.Serializable]
        public struct PredefinedBone
        {
            public HumanBodyBones boneType;
            public Transform boneTransform;
        }

        public PredefinedBone[] predefinedBones = new PredefinedBone[0];

        private readonly List<Transform> _srcTransforms = new List<Transform>();
        private TransformAccessArray _dstTransforms = new TransformAccessArray();

        private Dictionary<HumanBodyBones, Transform> _predefinedBonesDict;
        public Dictionary<HumanBodyBones, Transform> PredefinedBonesDict
        {
            get
            {
                if (_predefinedBonesDict == null)
                {
                    _predefinedBonesDict = new Dictionary<HumanBodyBones, Transform>();
                    for (int i = 0; i < predefinedBones.Length; ++i)
                    {
                        _predefinedBonesDict.Add(predefinedBones[i].boneType, predefinedBones[i].boneTransform);
                    }
                }
                return _predefinedBonesDict;
            }
        }

        public void PrepareTransforms(Animator src, Animator dst)
        {
#if !UNITY_SERVER
            if (src == null || dst == null)
                return;
            if (_dstTransforms.isCreated)
                _dstTransforms.Dispose();
            List<Transform> tempDstTransforms = new List<Transform>();
            for (int i = 0; i < (int)HumanBodyBones.LastBone; ++i)
            {
                Transform srcTransform = src.GetBoneTransform((HumanBodyBones)i);
                if (srcTransform == null)
                    continue;
                Transform dstTransform = null;
                try
                {
                    dstTransform = dst.GetBoneTransform((HumanBodyBones)i);
                }
                catch (System.Exception)
                {
                    // Some error occuring, skip it.
                }
                if (dstTransform != null)
                {
                    _srcTransforms.Add(srcTransform);
                    tempDstTransforms.Add(dstTransform);
                }
                else if (PredefinedBonesDict.TryGetValue((HumanBodyBones)i, out dstTransform))
                {
                    _srcTransforms.Add(srcTransform);
                    tempDstTransforms.Add(dstTransform);
                }
            }
            _dstTransforms = new TransformAccessArray(tempDstTransforms.ToArray());
#endif
        }

        private void OnDestroy()
        {
#if !UNITY_SERVER
            if (_dstTransforms.isCreated)
                _dstTransforms.Dispose();
#endif
        }

#if !UNITY_SERVER
        private void LateUpdate()
        {
            if (_srcTransforms.Count <= 0 || !_dstTransforms.isCreated)
                return;

            int transformsCount = _srcTransforms.Count;

            // Prepare source data
            NativeArray<Vector3> sourcePositions = new NativeArray<Vector3>(transformsCount, Allocator.TempJob);
            NativeArray<Quaternion> sourceRotations = new NativeArray<Quaternion>(transformsCount, Allocator.TempJob);
            NativeArray<Vector3> sourceLocalScales = new NativeArray<Vector3>(transformsCount, Allocator.TempJob);
            for (int i = 0; i < _srcTransforms.Count; ++i)
            {
                _srcTransforms[i].GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
                sourcePositions[i] = position;
                sourceRotations[i] = rotation;
                sourceLocalScales[i] = _srcTransforms[i].localScale;
            }

            // Prepare jobs
            CopyTransformsJob job = new CopyTransformsJob
            {
                sourcePositions = sourcePositions,
                sourceRotations = sourceRotations,
                sourceLocalScales = sourceLocalScales,
            };

            JobHandle jobHandle = job.Schedule(_dstTransforms);

            jobHandle.Complete();

            sourcePositions.Dispose();
            sourceRotations.Dispose();
            sourceLocalScales.Dispose();
        }
#endif

        [BurstCompile]
        private struct CopyTransformsJob : IJobParallelForTransform
        {
            [ReadOnly]
            public NativeArray<Vector3> sourcePositions;
            [ReadOnly]
            public NativeArray<Quaternion> sourceRotations;
            [ReadOnly]
            public NativeArray<Vector3> sourceLocalScales;

            public void Execute(int index, TransformAccess transform)
            {
                if (!transform.isValid)
                    return;
                transform.SetPositionAndRotation(sourcePositions[index], sourceRotations[index]);
                transform.localScale = sourceLocalScales[index];
            }
        }
    }
}
