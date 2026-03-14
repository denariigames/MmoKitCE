using Insthync.UnityEditorUtils.Editor;
using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(BaseGameEntity), true)]
    [CanEditMultipleObjects]
    public class BaseGameEntityEditor : BaseCustomCategorizedEditor
    {
    }
}
