using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class LanRpgClientUserContentHandlers : MonoBehaviour, IClientUserContentHandlers
    {
        public bool RequestUnlockContentProgression(RequestUnlockContentProgressionMessage data, ResponseDelegate<ResponseUnlockContentProgressionMessage> callback)
        {
            // Don't actual request, just do it in local device
            ResponseUnlockContentProgression(data, callback).Forget();
            return true;
        }

        private async UniTaskVoid ResponseUnlockContentProgression(RequestUnlockContentProgressionMessage data, ResponseDelegate<ResponseUnlockContentProgressionMessage> callback)
        {
            await UniTask.NextFrame();
            callback.Invoke(new ResponseHandlerData(0, null, 0, null), AckResponseCode.Success, new ResponseUnlockContentProgressionMessage()
            {
                message = UITextKeys.NONE,
                unlockableContent = new UnlockableContent()
                {
                    type = data.type,
                    dataId = data.dataId,
                    progression = 0,
                    unlocked = true,
                }
            });
        }

        public bool RequestAvailableContents(RequestAvailableContentsMessage data, ResponseDelegate<ResponseAvailableContentsMessage> callback)
        {
            // Don't actual request, just do it in local device
            ResponseAvailableContents(data, callback).Forget();
            return true;
        }

        private async UniTaskVoid ResponseAvailableContents(RequestAvailableContentsMessage data, ResponseDelegate<ResponseAvailableContentsMessage> callback)
        {
            await UniTask.NextFrame();
            List<UnlockableContent> contents = new List<UnlockableContent>();
            switch (data.type)
            {
                case UnlockableContentType.Icon:
                    foreach (PlayerIcon playerIcon in GameInstance.PlayerIcons.Values)
                    {
                        contents.Add(new UnlockableContent()
                        {
                            type = UnlockableContentType.Icon,
                            dataId = playerIcon.DataId,
                            progression = 0,
                            unlocked = true,
                        });
                    }
                    break;
                case UnlockableContentType.Frame:
                    foreach (PlayerFrame playerFrame in GameInstance.PlayerFrames.Values)
                    {
                        contents.Add(new UnlockableContent()
                        {
                            type = UnlockableContentType.Frame,
                            dataId = playerFrame.DataId,
                            progression = 0,
                            unlocked = true,
                        });
                    }
                    break;
                case UnlockableContentType.Title:
                    foreach (PlayerTitle playerTitle in GameInstance.PlayerTitles.Values)
                    {
                        contents.Add(new UnlockableContent()
                        {
                            type = UnlockableContentType.Title,
                            dataId = playerTitle.DataId,
                            progression = 0,
                            unlocked = true,
                        });
                    }
                    break;
            }
            callback.Invoke(default, AckResponseCode.Success, new ResponseAvailableContentsMessage()
            {
                message = UITextKeys.NONE,
                contents = contents.ToArray(),
            });
        }

        public bool RequestUnlockContent(RequestUnlockContentMessage data, ResponseDelegate<ResponseUnlockContentMessage> callback)
        {
            // Don't actual request, just do it in local device
            ResponseUnlockContent(data, callback).Forget();
            return true;
        }

        private async UniTaskVoid ResponseUnlockContent(RequestUnlockContentMessage data, ResponseDelegate<ResponseUnlockContentMessage> callback)
        {
            await UniTask.NextFrame();
            callback.Invoke(default, AckResponseCode.Success, new ResponseUnlockContentMessage()
            {
                message = UITextKeys.NONE,
            });
        }
    }
}
