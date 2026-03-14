using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseNpcDialogAction : ScriptableObject
    {
        public abstract UniTask<bool> IsPass(IPlayerCharacterData player);
        public abstract UniTask DoAction(IPlayerCharacterData player);
    }
}
