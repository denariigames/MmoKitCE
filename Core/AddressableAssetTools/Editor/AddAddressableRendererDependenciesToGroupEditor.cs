#if !DISABLE_ADDRESSABLES
using UnityEditor;
using UnityEngine;

namespace Insthync.AddressableAssetTools
{
    public class AddAddressableRendererDependenciesToGroupEditor : BaseAddAddressableToGroupEditor
    {
        protected bool _includeMeshes = true;
        protected bool _includeMaterials = true;
        protected bool _includeTextures = true;
        protected bool _includeSprites = true;
        protected bool _includeShaders = true;
        protected bool _includeAnimations = true;
        protected bool _includeModels = true;

        [MenuItem("Tools/Addressables/Add Renderer Dependencies to Group By Assets")]
        public static void ShowWindow()
        {
            GetWindow<AddAddressableRendererDependenciesToGroupEditor>("Add Renderer Dependencies to Group By Assets");
        }

        protected override void OnGUI_SelectedAssetsSection()
        {
            base.OnGUI_SelectedAssetsSection();
            _includeMeshes = EditorGUILayout.Toggle("Include Meshes", _includeMeshes);
            _includeMaterials = EditorGUILayout.Toggle("Include Materials", _includeMaterials);
            _includeTextures = EditorGUILayout.Toggle("Include Textures", _includeTextures);
            _includeSprites = EditorGUILayout.Toggle("Include Sprites", _includeSprites);
            _includeShaders = EditorGUILayout.Toggle("Include Shaders", _includeShaders);
            _includeAnimations = EditorGUILayout.Toggle("Include Animations", _includeAnimations);
            _includeModels = EditorGUILayout.Toggle("Include Models (FBX, OBJ)", _includeModels);
        }

        protected override bool IsTargetAsset(string dependencyPath)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(dependencyPath);

            if (obj is Mesh && _includeMeshes)
                return true;
            if (obj is Material && _includeMaterials)
                return true;
            if (obj is Texture && _includeTextures)
                return true;
            if (obj is Sprite && _includeSprites)
                return true;
            if (obj is Shader && _includeShaders)
                return true;
            if ((obj is AnimatorOverrideController || obj is AnimationClip) && _includeAnimations)
                return true;
            if (((obj is GameObject && dependencyPath.ToLower().EndsWith("fbx")) ||
                (obj is GameObject && dependencyPath.ToLower().EndsWith("obj"))) && _includeModels)
                return true;
            return false;
        }
    }
}
#endif