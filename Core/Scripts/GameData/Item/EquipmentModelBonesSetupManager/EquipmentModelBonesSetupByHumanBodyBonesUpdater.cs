using System.Collections.Generic;
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

        private List<Transform> _srcTransforms = new List<Transform>();
        private List<Transform> _dstTransforms = new List<Transform>();

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

            _srcTransforms.Clear();
            _dstTransforms.Clear();

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
                catch { }

                if (dstTransform != null ||
                    PredefinedBonesDict.TryGetValue((HumanBodyBones)i, out dstTransform))
                {
                    _srcTransforms.Add(srcTransform);
                    _dstTransforms.Add(dstTransform);
                }
            }
#endif
        }

        private void LateUpdate()
        {
#if !UNITY_SERVER
            // Register instead of scheduling job
            EquipmentModelBonesSetupByHumanBodyBonesUpdateManager.Instance.Register(_srcTransforms, _dstTransforms);
#endif
        }

        private void OnDestroy()
        {
#if !UNITY_SERVER
            _srcTransforms.Clear();
            _dstTransforms.Clear();
#endif
        }
    }
}