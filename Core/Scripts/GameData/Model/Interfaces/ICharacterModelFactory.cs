using UnityEngine;

namespace MultiplayerARPG
{
    public interface ICharacterModelFactory
    {
        string Name { get; }
        DimensionType DimensionType { get; }
        bool ValidateSourceObject(GameObject obj);
        BaseCharacterModel Setup(GameObject obj);
    }
}
