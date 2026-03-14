using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseGMCommands : ScriptableObject
    {
        /// <summary>
        /// Return `TRUE` if message contains GM command
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract bool IsGMCommand(string chatMessage, out string command);
        /// <summary>
        /// Return `TRUE` if character can use command
        /// </summary>
        /// <param name="userLevel"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract bool CanUseGMCommand(int userLevel, string command);
        /// <summary>
        /// Return response message, it's a message which send to command user.
        /// </summary>
        /// <param name="sender">Sender's name</param>
        /// <param name="senderCharacter">Sender's character entity</param>
        /// <param name="chatMessage">Message</param>
        /// <returns></returns>
        public abstract UniTask<string> HandleGMCommand(string sender, BasePlayerCharacterEntity senderCharacter, string chatMessage);
    }
}
