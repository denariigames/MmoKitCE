#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Insthync.AddressableAssetTools
{
    /// <summary>
    /// Creates an AssetReference that is restricted to having a specific Component.
    /// * This is the class that inherits from AssetReference.  It is generic and does not specify which Components it might care about.  A concrete child of this class is required for serialization to work.* At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At runtime it can load/instantiate the GameObject, then return the desired component.  API matches base class (LoadAssetAsync & InstantiateAsync).
    /// </summary>
    /// <typeparam name="TComponent"> The component type.</typeparam>
    public class AssetReferenceComponent<TComponent> : AssetReference
        where TComponent : Component
    {
        public AssetReferenceComponent(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(Object obj)
        {
            return ValidateAsset<TComponent>(obj);
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset<TComponent>(path);
        }

        protected bool ValidateAsset<T>(Object obj)
            where T : Component
        {
            GameObject go = obj as GameObject;
            return go != null && go.GetComponent<T>() != null;
        }

        protected bool ValidateAsset<T>(string path)
            where T : Component
        {
#if UNITY_EDITOR
            return ValidateAsset<T>(AssetDatabase.LoadAssetAtPath<GameObject>(path));
#else
            return false;
#endif
        }
    }
}
#endif