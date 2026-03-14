using UnityEngine;
using UnityEditor;

namespace Insthync.UnityEditorUtils.Editor
{
    public class FindMissingScriptsRecursively : BaseFindObjectsRecursivelyTool
    {
        [MenuItem("Window/Object Finding Tools/Find Missing Scripts")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScriptsRecursively));
        }

        public override string GetObjectName()
        {
            return "missing script(s)";
        }

        protected override bool IsTargetObject(Component comp)
        {
            return comp == null;
        }
    }
}
