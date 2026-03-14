using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseMessageManager : ScriptableObject
    {
        public abstract string ReplaceKeysToMessages(string format);
    }
}