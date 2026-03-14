using Cysharp.Threading.Tasks;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerUserContentHandlers
    {
        UniTask<System.ValueTuple<UITextKeys, UnlockableContent>> FillUserContentProgressionForUnlocking(string userId, UnlockableContentType type, int dataId);
        UniTask<System.ValueTuple<UITextKeys, UnlockableContent>> ChangeUnlockUserContentProgress(string userId, UnlockableContentType type, int dataId, int changeProgress);
        UniTask<System.ValueTuple<UITextKeys, UnlockableContent>> GetUnlockContentProgression(string userId, UnlockableContentType type, int dataId);
        UniTask<System.ValueTuple<UITextKeys, UnlockableContent[]>> GetAvailableContents(string userId, UnlockableContentType type);
    }
}