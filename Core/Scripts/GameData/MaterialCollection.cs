using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class MaterialCollection
    {
        [Tooltip("Renderer which will be applied by the materials")]
        public Renderer renderer;
        [Tooltip("Materials which will be applied to the renderer")]
        public Material[] materials;

        public void Apply()
        {
            if (renderer == null || materials == null || materials.Length == 0)
                return;
            renderer.materials = materials;
        }
    }

    public static class MaterialCollectionExtensions
    {
        public static void ApplyMaterials(this MaterialCollection[] materials)
        {
            if (materials == null || materials.Length == 0)
                return;
            foreach (MaterialCollection material in materials)
            {
                material.Apply();
            }
        }
    }
}
