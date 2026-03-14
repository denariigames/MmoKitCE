using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseGameSaveSystem : ScriptableObject
    {
        public abstract void OnServerStart();
        public abstract UniTask PreSpawnEntities(IPlayerCharacterData hostPlayerCharacterData, IDictionary<StorageId, List<CharacterItem>> storageItems);
        public abstract void SaveWorld(IPlayerCharacterData hostPlayerCharacterData, IEnumerable<IBuildingSaveData> buildings);
        public abstract void SaveStorage(IPlayerCharacterData hostPlayerCharacterData, IDictionary<StorageId, List<CharacterItem>> storageItems);
        public abstract void SavePlayerStorage(IPlayerCharacterData playerCharacterData, List<CharacterItem> storageItems);
        public abstract void SaveCharacter(IPlayerCharacterData playerCharacterData);
        public abstract void SaveSummonBuffs(IPlayerCharacterData playerCharacterData, List<CharacterSummon> summons);
        public abstract List<PlayerCharacterData> LoadCharacters();
        public abstract List<CharacterBuff> LoadSummonBuffs(IPlayerCharacterData playerCharacterData);
        public abstract List<CharacterItem> LoadPlayerStorage(IPlayerCharacterData playerCharacterData);
        public abstract void OnSceneChanging();
    }
}
