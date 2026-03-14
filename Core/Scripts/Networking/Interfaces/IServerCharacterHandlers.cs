using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IServerCharacterHandlers
    {
        void HandleRequestOnlineCharacter(MessageHandlerData messageHandler);
        void MarkOnlineCharacter(string characterId);
        void ClearOnlineCharacters();
        UniTask Respawn(int option, IPlayerCharacterData playerCharacter);
    }
}
