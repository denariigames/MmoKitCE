using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class SyncFieldEquipWeapons : LiteNetLibSyncField<EquipWeapons>
    {
        protected override bool IsValueChanged(EquipWeapons oldValue, EquipWeapons newValue)
        {
            return true;
        }
    }


    [System.Serializable]
    public class SyncListEquipWeapons : LiteNetLibSyncList<EquipWeapons>
    {
    }
}
