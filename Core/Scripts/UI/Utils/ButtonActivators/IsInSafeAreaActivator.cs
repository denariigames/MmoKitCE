using UnityEngine;

namespace MultiplayerARPG {
    public class IsInSafeAreaActivator : MonoBehaviour
    {
        public GameObject[] activateObjects = new GameObject[0];

        private DamageableEntity _damageableEntity;

        private void Start()
        {
            _damageableEntity = GetComponentInParent<DamageableEntity>();
            if (_damageableEntity == null)
            {
                GameInstance.OnSetPlayingCharacterEvent += GameInstance_onSetPlayingCharacter;
                GameInstance_onSetPlayingCharacter(GameInstance.PlayingCharacterEntity);
            }
        }

        private void OnDestroy()
        {
            GameInstance.OnSetPlayingCharacterEvent -= GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(null);
        }

        private void GameInstance_onSetPlayingCharacter(IPlayerCharacterData playingCharacterData)
        {
            _damageableEntity = playingCharacterData as BasePlayerCharacterEntity;
        }

        private void LateUpdate()
        {
            bool isInSafeArea = _damageableEntity.IsInSafeArea;
            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(isInSafeArea);
            }
        }
    }
}