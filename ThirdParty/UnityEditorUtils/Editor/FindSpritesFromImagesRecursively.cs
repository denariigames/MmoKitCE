using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Insthync.UnityEditorUtils.Editor
{
    public class FindSpritesFromImagesRecursively : BaseFindObjectsRecursivelyTool
    {
        protected Sprite _findingSprite = null;

        [MenuItem("Window/Object Finding Tools/Find Sprites From Images")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindSpritesFromImagesRecursively));
        }

        public override string GetObjectName()
        {
            return "sprites(s)";
        }

        protected override bool IsTargetObject(Component comp)
        {
            return comp is Image image && image.sprite == _findingSprite;
        }

        protected override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            _findingSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", _findingSprite, typeof(Sprite), true);
            GUILayout.EndHorizontal();
            base.OnGUI();
        }
    }
}