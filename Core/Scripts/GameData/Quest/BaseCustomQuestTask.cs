using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class BaseCustomQuestTask : ScriptableObject
    {
        /// <summary>
        /// Task description which will show in `UIQuestTask` -> `uiTextTaskDescription`
        /// </summary>
        /// <param name="playerCharacter"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public abstract string GetTaskDescription(IPlayerCharacterData playerCharacter, int progress);
        public abstract int GetTaskProgress(IPlayerCharacterData playerCharacter, Quest quest, int taskIndex, out string targetTitle, out int maxProgress, out bool isComplete);
    }
}
