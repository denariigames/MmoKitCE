#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        private void CreateCharacterItem(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, InventoryType inventoryType, CharacterItem characterItem)
        {
            string id = characterItem.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Item {id}, inventory type {inventoryType}, for character {characterId}, already inserted");
                return;
            }
            if (string.IsNullOrEmpty(id))
                return;
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO characteritem (id, idx, inventoryType, characterId, dataId, level, amount, equipSlotIndex, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version) VALUES (@id, @idx, @inventoryType, @characterId, @dataId, @level, @amount, @equipSlotIndex, @durability, @exp, @lockRemainsDuration, @expireTime, @randomSeed, @ammo, @sockets, @version)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@idx", idx),
                new SqliteParameter("@inventoryType", (byte)inventoryType),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@dataId", characterItem.dataId),
                new SqliteParameter("@level", characterItem.level),
                new SqliteParameter("@amount", characterItem.amount),
                new SqliteParameter("@equipSlotIndex", characterItem.equipSlotIndex),
                new SqliteParameter("@durability", characterItem.durability),
                new SqliteParameter("@exp", characterItem.exp),
                new SqliteParameter("@lockRemainsDuration", characterItem.lockRemainsDuration),
                new SqliteParameter("@expireTime", characterItem.expireTime),
                new SqliteParameter("@randomSeed", characterItem.randomSeed),
                new SqliteParameter("@ammo", characterItem.ammo),
                new SqliteParameter("@sockets", characterItem.WriteSockets()),
                new SqliteParameter("@version", characterItem.version));
        }

        private bool GetCharacterItem(SqliteDataReader reader, out CharacterItem result)
        {
            if (reader.Read())
            {
                result = new CharacterItem();
                result.id = reader.GetString(0);
                result.dataId = reader.GetInt32(1);
                result.level = reader.GetInt32(2);
                result.amount = reader.GetInt32(3);
                result.equipSlotIndex = reader.GetByte(4);
                result.durability = reader.GetFloat(5);
                result.exp = reader.GetInt32(6);
                result.lockRemainsDuration = reader.GetFloat(7);
                result.expireTime = reader.GetInt64(8);
                result.randomSeed = reader.GetInt32(9);
                result.ammo = reader.GetInt32(10);
                result.ReadSockets(reader.GetString(11));
                result.version = reader.GetByte(12);
                return true;
            }
            result = CharacterItem.Empty;
            return false;
        }

        private List<CharacterItem> ReadCharacterItems(string characterId, InventoryType inventoryType, List<CharacterItem> result = null)
        {
            if (result == null)
                result = new List<CharacterItem>();
            ExecuteReader((reader) =>
            {
                CharacterItem tempInventory;
                while (GetCharacterItem(reader, out tempInventory))
                {
                    result.Add(tempInventory);
                }
            }, "SELECT id, dataId, level, amount, equipSlotIndex, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version FROM characteritem WHERE characterId=@characterId AND inventoryType=@inventoryType ORDER BY idx ASC",
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@inventoryType", (byte)inventoryType));
            return result;
        }

        public List<EquipWeapons> ReadCharacterEquipWeapons(string characterId, List<EquipWeapons> result = null)
        {
            if (result == null)
                result = new List<EquipWeapons>();
            ExecuteReader((reader) =>
            {
                EquipWeapons tempEquipWeapons;
                CharacterItem tempInventory;
                byte equipWeaponSet;
                InventoryType inventoryType;
                while (GetCharacterItem(reader, out tempInventory))
                {
                    equipWeaponSet = reader.GetByte(13);
                    inventoryType = (InventoryType)reader.GetByte(14);
                    // Fill weapon sets if needed
                    while (result.Count <= equipWeaponSet)
                        result.Add(new EquipWeapons());
                    // Get equip weapon set
                    if (inventoryType == InventoryType.EquipWeaponRight)
                    {
                        tempEquipWeapons = result[equipWeaponSet];
                        tempEquipWeapons.rightHand = tempInventory;
                        result[equipWeaponSet] = tempEquipWeapons;
                    }
                    if (inventoryType == InventoryType.EquipWeaponLeft)
                    {
                        tempEquipWeapons = result[equipWeaponSet];
                        tempEquipWeapons.leftHand = tempInventory;
                        result[equipWeaponSet] = tempEquipWeapons;
                    }
                }
            }, "SELECT id, dataId, level, amount, equipSlotIndex, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version, idx, inventoryType FROM characteritem WHERE characterId=@characterId AND (inventoryType=@inventoryType1 OR inventoryType=@inventoryType2) ORDER BY idx ASC",
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@inventoryType1", (byte)InventoryType.EquipWeaponRight),
                new SqliteParameter("@inventoryType2", (byte)InventoryType.EquipWeaponLeft));
            return result;
        }

        public void CreateCharacterEquipWeapons(SqliteTransaction transaction, HashSet<string> insertedIds, int equipWeaponSet, string characterId, EquipWeapons equipWeapons)
        {
            CreateCharacterItem(transaction, insertedIds, equipWeaponSet, characterId, InventoryType.EquipWeaponRight, equipWeapons.rightHand);
            CreateCharacterItem(transaction, insertedIds, equipWeaponSet, characterId, InventoryType.EquipWeaponLeft, equipWeapons.leftHand);
        }

        public void CreateCharacterEquipItem(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterItem characterItem)
        {
            CreateCharacterItem(transaction, insertedIds, idx, characterId, InventoryType.EquipItems, characterItem);
        }

        public List<CharacterItem> ReadCharacterEquipItems(string characterId, List<CharacterItem> result = null)
        {
            return ReadCharacterItems(characterId, InventoryType.EquipItems, result);
        }

        public void CreateCharacterNonEquipItem(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterItem characterItem)
        {
            CreateCharacterItem(transaction, insertedIds, idx, characterId, InventoryType.NonEquipItems, characterItem);
        }

        public List<CharacterItem> ReadCharacterNonEquipItems(string characterId, List<CharacterItem> result = null)
        {
            return ReadCharacterItems(characterId, InventoryType.NonEquipItems, result);
        }

        public void DeleteCharacterItems(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM characteritem WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }

        public void DeleteCharacterItems(SqliteTransaction transaction, InventoryType inventoryType, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM characteritem WHERE inventoryType=@inventoryType AND characterId=@characterId",
                new SqliteParameter("@inventoryType", (byte)inventoryType),
                new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif