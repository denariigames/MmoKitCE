using UnityEngine;

namespace MultiplayerARPG
{
    public struct UICharacterAttributeData
    {
        public CharacterAttribute characterAttribute;
        public float targetAmount;
        public UICharacterAttributeData(CharacterAttribute characterAttribute, float targetAmount)
        {
            this.characterAttribute = characterAttribute;
            this.targetAmount = targetAmount;
        }

        public UICharacterAttributeData(CharacterAttribute characterAttribute) : this(characterAttribute, characterAttribute.amount)
        {
        }

        public UICharacterAttributeData(Attribute attribute, int targetLevel) : this(CharacterAttribute.Create(attribute, targetLevel), targetLevel)
        {
        }

        public UICharacterAttributeData(Attribute attribute, float targetLevel) : this(CharacterAttribute.Create(attribute, Mathf.CeilToInt(targetLevel)), targetLevel)
        {
        }
    }
}
