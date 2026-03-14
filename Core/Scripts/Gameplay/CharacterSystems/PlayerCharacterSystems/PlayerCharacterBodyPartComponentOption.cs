using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.PLAYER_CHARACTER_BODY_PART_COMPONENT_OPTION_FILE, menuName = GameDataMenuConsts.PLAYER_CHARACTER_BODY_PART_COMPONENT_OPTION_MENU, order = GameDataMenuConsts.PLAYER_CHARACTER_BODY_PART_COMPONENT_OPTION_ORDER)]
    public class PlayerCharacterBodyPartComponentOption : ScriptableObject
    {
        public PlayerCharacterBodyPartComponent.ModelOption data;
    }
}