using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseCustomNpcDialogCondition : ScriptableObject
    {
        public abstract UniTask<bool> IsPass(IPlayerCharacterData player);
    }
}
