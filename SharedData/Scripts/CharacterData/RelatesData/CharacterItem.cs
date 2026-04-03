using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Flags]
    internal enum CharacterItemSyncState : byte
    {
        None = 0,
        IsEquipment = 1 << 1,
        IsWeapon = 1 << 2,
        IsPet = 1 << 3,
        IsEmpty = 1 << 4,
    }

    internal static class CharacterItemSyncStateExtensions
    {
        internal static bool Has(this CharacterItemSyncState self, CharacterItemSyncState flag)
        {
            return (self & flag) == flag;
        }
    }

    [System.Serializable]
    public partial struct CharacterItem : INetSerializable
    {
        public static readonly CharacterItem Empty = new CharacterItem();
        public string id;
        public int dataId;
        public int level;
        public int amount;
        public byte equipSlotIndex;
        public float durability;
        public int exp;
        public float lockRemainsDuration;
        public long expireTime;
        public int randomSeed;
        public int ammoDataId;
        public int ammo;
        public List<int> sockets;
        public byte version;

        public List<int> ReadSockets(string socketsString, char separator = ';')
        {
            sockets = socketsString.ReadCharacterItemSockets(separator);
            return sockets;
        }

        public string WriteSockets(char separator = ';')
        {
            return sockets.WriteCharacterItemSockets(separator);
        }

        public CharacterItem Clone(bool generateNewId = false)
        {
            List<int> sockets = this.sockets == null ? new List<int>() : new List<int>(this.sockets);
            CharacterItem destination = new CharacterItem()
            {
                id = generateNewId || string.IsNullOrWhiteSpace(id) ? GenericUtils.GetUniqueId() : id,
                dataId = dataId,
                level = level,
                amount = amount,
                equipSlotIndex = equipSlotIndex,
                durability = durability,
                exp = exp,
                lockRemainsDuration = lockRemainsDuration,
                expireTime = expireTime,
                randomSeed = randomSeed,
                ammoDataId = ammoDataId,
                ammo = ammo,
                sockets = sockets,
                version = version,
            };
            if (GameExtensionInstance.onCharacterItemClone == null)
                return destination;
            return GameExtensionInstance.onCharacterItemClone(ref this, destination);
        }

        public void Serialize(NetDataWriter writer)
        {
            if (amount <= 0 || dataId == 0)
            {
                writer.Put((byte)CharacterItemSyncState.IsEmpty);
                writer.Put(id);
                GameExtensionInstance.onCharacterItemSerialize?.Invoke(ref this, writer);
                return;
            }

            // Unknow item data syncing changed to sync all data
            bool isUnknowItem = GetItem() == null;
            bool isEquipment = isUnknowItem || GetEquipmentItem() != null;
            bool isWeapon = isUnknowItem || isEquipment && GetWeaponItem() != null;
            bool isPet = isUnknowItem || GetPetItem() != null;
            CharacterItemSyncState syncState = CharacterItemSyncState.None;
            if (isEquipment)
            {
                syncState |= CharacterItemSyncState.IsEquipment;
            }
            if (isWeapon)
            {
                syncState |= CharacterItemSyncState.IsWeapon;
            }
            if (isPet)
            {
                syncState |= CharacterItemSyncState.IsPet;
            }
            writer.Put((byte)syncState);

            writer.Put(id);
            writer.PutPackedLong(expireTime);
            writer.PutPackedInt(dataId);
            writer.PutPackedInt(level);
            writer.PutPackedInt(amount);
            writer.Put(equipSlotIndex);
            writer.Put(lockRemainsDuration);

            if (isEquipment)
            {
                writer.Put(durability);
                writer.PutPackedInt(exp);

                byte socketCount = 0;
                if (sockets != null)
                    socketCount = (byte)sockets.Count;
                writer.Put(socketCount);
                if (socketCount > 0)
                {
                    foreach (int socketDataId in sockets)
                    {
                        writer.PutPackedInt(socketDataId);
                    }
                }

                writer.PutPackedInt(randomSeed);
            }

            if (isWeapon)
            {
                writer.PutPackedInt(ammoDataId);
                writer.PutPackedInt(ammo);
            }

            if (isPet)
            {
                writer.PutPackedInt(exp);
            }

            writer.Put(version);
            GameExtensionInstance.onCharacterItemSerialize?.Invoke(ref this, writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            if (sockets == null)
                sockets = new List<int>();

            CharacterItemSyncState syncState = (CharacterItemSyncState)reader.GetByte();
            if (syncState == CharacterItemSyncState.IsEmpty)
            {
                id = reader.GetString();
                dataId = 0;
                level = 0;
                amount = 0;
                equipSlotIndex = 0;
                durability = 0;
                exp = 0;
                lockRemainsDuration = 0;
                expireTime = 0;
                randomSeed = 0;
                ammoDataId = 0;
                ammo = 0;
                sockets.Clear();
                GameExtensionInstance.onCharacterItemDeserialize?.Invoke(ref this, reader);
                return;
            }

            id = reader.GetString();
            expireTime = reader.GetPackedLong();
            dataId = reader.GetPackedInt();
            level = reader.GetPackedInt();
            amount = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
            lockRemainsDuration = reader.GetFloat();

            if (syncState.Has(CharacterItemSyncState.IsEquipment))
            {
                durability = reader.GetFloat();
                exp = reader.GetPackedInt();

                byte socketCount = reader.GetByte();
                sockets.Clear();
                for (byte i = 0; i < socketCount; ++i)
                {
                    sockets.Add(reader.GetPackedInt());
                }

                randomSeed = reader.GetPackedInt();
            }

            if (syncState.Has(CharacterItemSyncState.IsWeapon))
            {
                ammoDataId = reader.GetPackedInt();
                ammo = reader.GetPackedInt();
            }

            if (syncState.Has(CharacterItemSyncState.IsPet))
            {
                exp = reader.GetPackedInt();
            }

            version = reader.GetByte();
            GameExtensionInstance.onCharacterItemDeserialize?.Invoke(ref this, reader);
        }
    }
}
