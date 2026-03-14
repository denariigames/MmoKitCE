using Insthync.UnityEditorUtils.Editor;
using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(BaseGameData), true)]
    [CanEditMultipleObjects]
    public class BaseGameDataEditor : BaseCustomCategorizedEditor
    {
    }
}
