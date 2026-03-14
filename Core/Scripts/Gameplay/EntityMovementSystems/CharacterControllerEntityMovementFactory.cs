using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterControllerEntityMovementFactory : IEntityMovementFactory
    {
        public string Name => "Character Controller Entity Movement";

        public DimensionType DimensionType => DimensionType.Dimension3D;

        public CharacterControllerEntityMovementFactory()
        {

        }

        public bool ValidateSourceObject(GameObject obj)
        {
            return true;
        }

        public IEntityMovementComponent Setup(GameObject obj, ref Bounds bounds)
        {
            bounds = default;
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshes.Length; ++i)
            {
                if (i > 0)
                    bounds.Encapsulate(meshes[i].bounds);
                else
                    bounds = meshes[i].bounds;
            }

            SkinnedMeshRenderer[] skinnedMeshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshes.Length; ++i)
            {
                if (i > 0)
                    bounds.Encapsulate(skinnedMeshes[i].bounds);
                else
                    bounds = skinnedMeshes[i].bounds;
            }

            float scale = Mathf.Max(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
            bounds.size = bounds.size / scale;
            bounds.center = bounds.center / scale;

            CharacterController characterController = obj.AddComponent<CharacterController>();
            characterController.height = bounds.size.y;
            characterController.radius = Mathf.Min(bounds.extents.x, bounds.extents.z);
            characterController.center = Vector3.zero + (Vector3.up * characterController.height * 0.5f);

            return obj.AddComponent<CharacterControllerEntityMovement>();
        }
    }
}
