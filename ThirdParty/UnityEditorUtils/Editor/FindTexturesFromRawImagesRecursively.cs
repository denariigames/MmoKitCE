using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Insthync.UnityEditorUtils.Editor
{
    public class FindTexturesFromRawImagesRecursively : BaseFindObjectsRecursivelyTool
    {
        protected Texture _findingTexture = null;

        [MenuItem("Window/Object Finding Tools/Find Textures From Raw Images")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindTexturesFromRawImagesRecursively));
        }

        public override string GetObjectName()
        {
            return "texture(s)";
        }

        protected override bool IsTargetObject(Component comp)
        {
            return comp is RawImage rawImage && rawImage.texture == _findingTexture;
        }

        protected override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            _findingTexture = (Texture)EditorGUILayout.ObjectField("Texture", _findingTexture, typeof(Texture), true);
            GUILayout.EndHorizontal();
            base.OnGUI();
        }
    }
}