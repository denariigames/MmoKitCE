using Cysharp.Threading.Tasks;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerMailHandlers
    {
        UniTask<bool> SendMail(Mail mail);
    }
}
