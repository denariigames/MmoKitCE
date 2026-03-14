using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseGameDatabase : ScriptableObject
    {
        public async UniTaskVoid LoadData(GameInstance gameInstance)
        {
            await LoadDataImplement(gameInstance);
            // Tell game instance that data loaded
            gameInstance.LoadedGameData();
        }
        protected abstract UniTask LoadDataImplement(GameInstance gameInstance);
    }
}
