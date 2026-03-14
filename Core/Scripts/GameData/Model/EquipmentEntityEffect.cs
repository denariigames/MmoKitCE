using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class EquipmentEntityEffect : System.IComparable<EquipmentEntityEffect>
    {
        public int level = 1;
        [HideInInspector]
        [System.Obsolete("This is deprecated, use `effectMaterials` instead.")]
        public Material[] materials;
        [FormerlySerializedAs("equipmentMaterials")]
        public MaterialCollection[] visibleMaterials = new MaterialCollection[0];
        public GameObject[] effectObjects = new GameObject[0];

        public int CompareTo(EquipmentEntityEffect other)
        {
            return level.CompareTo(other.level);
        }

        public void ApplyMaterials()
        {
            visibleMaterials.ApplyMaterials();
        }
    }
}
