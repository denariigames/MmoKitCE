using Insthync.UnityEditorUtils.Editor;
using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(GameInstance))]
    [CanEditMultipleObjects]
    public class GameInstanceEditor : BaseCustomEditor
    {
        protected override void SetFieldCondition()
        {
            ShowOnBool("HasNewCharacterSetting", false, "startGold");
            ShowOnBool("HasNewCharacterSetting", false, "startItems");
        }
    }
}
