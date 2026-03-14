using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class ItemRequirement
    {
        [Header("Class")]
        [FormerlySerializedAs("character")]
        [Tooltip("Which character classes can equip item. This is a part of `availableClasses`, just keep it for backward compatibility.")]
        public PlayerCharacter availableClass = null;
        [Tooltip("Which character classes can equip item.")]
        public List<PlayerCharacter> availableClasses = new List<PlayerCharacter>();

        [Header("Faction")]
        [Tooltip("Which character factions can equip item.")]
        public List<Faction> availableFactions = new List<Faction>();

        [Header("Level and Attributes")]
        [Tooltip("Character must have level equals or more than this setting to equip item.")]
        public int level = 0;
        [Tooltip("Character must have attribute amounts equals or more than this setting to equip item.")]
        [ArrayElementTitle("attribute")]
        public AttributeAmount[] attributeAmounts = new AttributeAmount[0];

        public bool HasAvailableClasses()
        {
            return availableClass != null || (availableClasses != null && availableClasses.Count > 0);
        }

        public bool ClassIsAvailable(int dataId)
        {
            if (!HasAvailableClasses())
                return true;
            if (availableClass != null && availableClass.DataId == dataId)
                return true;
            if (availableClasses != null && availableClasses.Count > 0)
            {
                for (int i = 0; i < availableClasses.Count; ++i)
                {
                    if (availableClasses[i] != null && availableClasses[i].DataId == dataId)
                        return true;
                }
            }
            return false;
        }

        public bool HasAvailableFactions()
        {
            return availableFactions != null && availableFactions.Count > 0;
        }

        public bool FactionIsAvailable(int dataId)
        {
            if (!HasAvailableFactions())
                return true;
            if (availableFactions != null && availableFactions.Count > 0)
            {
                for (int i = 0; i < availableFactions.Count; ++i)
                {
                    if (availableFactions[i] != null && availableFactions[i].DataId == dataId)
                        return true;
                }
            }
            return false;
        }
    }
}
