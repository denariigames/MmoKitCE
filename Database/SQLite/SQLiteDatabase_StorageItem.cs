#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        private void CreateStorageItem(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, StorageType storageType, string storageOwnerId, CharacterItem characterItem)
        {
            string id = characterItem.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Storage item {id}, storage type {storageType}, owner {storageOwnerId}, already inserted");
                return;
            }
            if (string.IsNullOrEmpty(characterItem.id))
                return;
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO storageitem (id, idx, storageType, storageOwnerId, dataId, level, amount, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version) VALUES (@id, @idx, @storageType, @storageOwnerId, @dataId, @level, @amount, @durability, @exp, @lockRemainsDuration, @expireTime, @randomSeed, @ammo, @sockets, @version)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@idx", idx),
                new SqliteParameter("@storageType", (byte)storageType),
                new SqliteParameter("@storageOwnerId", storageOwnerId),
                new SqliteParameter("@dataId", characterItem.dataId),
                new SqliteParameter("@level", characterItem.level),
                new SqliteParameter("@amount", characterItem.amount),
                new SqliteParameter("@durability", characterItem.durability),
                new SqliteParameter("@exp", characterItem.exp),
                new SqliteParameter("@lockRemainsDuration", characterItem.lockRemainsDuration),
                new SqliteParameter("@expireTime", characterItem.expireTime),
                new SqliteParameter("@randomSeed", characterItem.randomSeed),
                new SqliteParameter("@ammo", characterItem.ammo),
                new SqliteParameter("@sockets", characterItem.WriteSockets()),
                new SqliteParameter("@version", characterItem.version));
        }

        private bool GetStorageItem(SqliteDataReader reader, out CharacterItem result)
        {
            if (reader.Read())
            {
                result = new CharacterItem();
                result.id = reader.GetString(0);
                result.dataId = reader.GetInt32(1);
                result.level = reader.GetInt32(2);
                result.amount = reader.GetInt32(3);
                result.durability = reader.GetFloat(4);
                result.exp = reader.GetInt32(5);
                result.lockRemainsDuration = reader.GetFloat(6);
                result.expireTime = reader.GetInt64(7);
                result.randomSeed = reader.GetInt32(8);
                result.ammo = reader.GetInt32(9);
                result.ReadSockets(reader.GetString(10));
                result.version = reader.GetByte(11);
                return true;
            }
            result = CharacterItem.Empty;
            return false;
        }

        public override UniTask<List<CharacterItem>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            List<CharacterItem> result = new List<CharacterItem>();
            ExecuteReader((reader) =>
            {
                CharacterItem tempInventory;
                while (GetStorageItem(reader, out tempInventory))
                {
                    result.Add(tempInventory);
                }
            }, "SELECT id, dataId, level, amount, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version FROM storageitem WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId ORDER BY idx ASC",
                new SqliteParameter("@storageType", (byte)storageType),
                new SqliteParameter("@storageOwnerId", storageOwnerId));
            return new UniTask<List<CharacterItem>>(result);
        }

        public override UniTask UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            using (SqliteTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    FillStorageItems(transaction, storageType, storageOwnerId, storageItems);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return new UniTask();
        }

        public override UniTask UpdateStorageAndCharacterItems(
            StorageType storageType,
            string storageOwnerId,
            List<CharacterItem> storageItems,
            string characterId,
            List<EquipWeapons> selectableWeaponSets,
            List<CharacterItem> equipItems,
            List<CharacterItem> nonEquipItems)
        {
            using (SqliteTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    FillStorageItems(transaction, storageType, storageOwnerId, storageItems);
                    FillCharacterItems(transaction, characterId, selectableWeaponSets, equipItems, nonEquipItems);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return new UniTask();
        }

        private void FillStorageItems(SqliteTransaction transaction, StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            try
            {
                DeleteStorageItems(transaction, storageType, storageOwnerId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < storageItems.Count; ++i)
                {
                    CreateStorageItem(transaction, insertedIds, i, storageType, storageOwnerId, storageItems[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing storage items");
                LogException(LogTag, ex);
                throw;
            }
        }

        public void DeleteStorageItems(SqliteTransaction transaction, StorageType storageType, string storageOwnerId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM storageitem WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                new SqliteParameter("@storageType", (byte)storageType),
                new SqliteParameter("@storageOwnerId", storageOwnerId));
        }

        public override UniTask<long> FindReservedStorage(StorageType storageType, string storageOwnerId)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM storage_reservation WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                new SqliteParameter("@storageType", (byte)storageType),
                new SqliteParameter("@storageOwnerId", storageOwnerId));
            return new UniTask<long>(result != null ? (long)result : 0);
        }

        public override UniTask UpdateReservedStorage(StorageType storageType, string storageOwnerId, string reserverId)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteNonQuery(transaction, "DELETE FROM storage_reservation WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                    new SqliteParameter("@storageType", (byte)storageType),
                    new SqliteParameter("@storageOwnerId", storageOwnerId));
                ExecuteNonQuery(transaction, "INSERT INTO storage_reservation (storageType, storageOwnerId, reserverId) VALUES (@storageType, @storageOwnerId, @reserverId)",
                    new SqliteParameter("@storageType", (byte)storageType),
                    new SqliteParameter("@storageOwnerId", storageOwnerId),
                    new SqliteParameter("@reserverId", reserverId));
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing storage reserving");
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            return new UniTask();
        }

        public override UniTask DeleteReservedStorage(StorageType storageType, string storageOwnerId)
        {
            ExecuteNonQuery("DELETE FROM storage_reservation WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                new SqliteParameter("@storageType", (byte)storageType),
                new SqliteParameter("@storageOwnerId", storageOwnerId));
            return new UniTask();
        }

        public override UniTask DeleteReservedStorageByReserver(string reserverId)
        {
            ExecuteNonQuery("DELETE FROM storage_reservation WHERE reserverId=@reserverId",
                new SqliteParameter("@reserverId", reserverId));
            return new UniTask();
        }

        public override UniTask DeleteAllReservedStorage()
        {
            ExecuteNonQuery("DELETE FROM storage_reservation");
            return new UniTask();
        }
    }
}
#endif