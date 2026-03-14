using UnityEngine;

namespace MultiplayerARPG.GameData.Model.Playables
{
    public class PlayableCharacterModelFactory : ICharacterModelFactory
    {
        public string Name => "Playable Character Model";
        public DimensionType DimensionType => DimensionType.Dimension3D;

        public PlayableCharacterModelFactory()
        {

        }

        public bool ValidateSourceObject(GameObject obj)
        {
            Animator comp = obj.GetComponentInChildren<Animator>();
            if (comp == null)
            {
                Debug.LogError("Cannot create new entity with `PlayableCharacterModel`, can't find `Animator` component");
                Object.DestroyImmediate(obj);
                return false;
            }
            return true;
        }

        public BaseCharacterModel Setup(GameObject obj)
        {
            PlayableCharacterModel characterModel = obj.AddComponent<PlayableCharacterModel>();
            characterModel.animator = obj.GetComponentInChildren<Animator>();
            return characterModel;
        }
    }
}
