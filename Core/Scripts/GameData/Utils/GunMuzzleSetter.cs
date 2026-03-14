using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class GunMuzzleSetter : MonoBehaviour
    {
        public GameEffect effect;
        public Vector3 effectAngles;
        public BaseEquipmentEntity[] equipmentEntities;

#if UNITY_EDITOR
        [ContextMenu("Set Muzzle")]
        public void SetMuzzle()
        {
            foreach (BaseEquipmentEntity entity in equipmentEntities)
            {
                GameEffect newEffect = Instantiate(effect, entity.transform);
                newEffect.transform.position = entity.missileDamageTransform.position;
                newEffect.transform.localScale = Vector3.one;
                newEffect.transform.localEulerAngles = effectAngles;
                List<GameEffect> list = new List<GameEffect>(entity.weaponLaunchEffects);
                list.Add(newEffect);
                entity.weaponLaunchEffects = list.ToArray();
                EditorUtility.SetDirty(entity);
            }
        }
#endif
    }
}
