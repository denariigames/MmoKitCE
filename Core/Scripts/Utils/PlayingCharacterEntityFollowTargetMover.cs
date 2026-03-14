using UnityEngine;

namespace MultiplayerARPG
{
    public class PlayingCharacterEntityFollowTargetMover : MonoBehaviour
    {
        public Transform targetTransform;

        private void LateUpdate()
        {
            if (GameInstance.PlayingCharacterEntity == null)
                return;
            GameInstance.PlayingCharacterEntity.transform.position = targetTransform.position;
            GameInstance.PlayingCharacterEntity.transform.rotation = targetTransform.rotation;
        }
    }
}