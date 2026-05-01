#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        public async UniTask CreateStorageItem(MySqlConnection connection, MySqlTransaction transaction, HashSet<string> insertedIds, int idx, StorageType storageType, string storageOwnerId, CharacterItem characterItem)
        {
            string id = characterItem.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Storage item {id}, storage type {storageType}, owner {storageOwnerId}, already inserted");
                return;
            }
            if (string.IsNullOrEmpty(id))
                return;
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO storageitem (id, idx, storageType, storageOwnerId, dataId, level, amount, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version) VALUES (@id, @idx, @storageType, @storageOwnerId, @dataId, @level, @amount, @durability, @exp, @lockRemainsDuration, @expireTime, @randomSeed, @ammo, @sockets, @version)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@idx", idx),
                new MySqlParameter("@storageType", (byte)storageType),
                new MySqlParameter("@storageOwnerId", storageOwnerId),
                new MySqlParameter("@dataId", characterItem.dataId),
                new MySqlParameter("@level", characterItem.level),
                new MySqlParameter("@amount", characterItem.amount),
                new MySqlParameter("@durability", characterItem.durability),
                new MySqlParameter("@exp", characterItem.exp),
                new MySqlParameter("@lockRemainsDuration", characterItem.lockRemainsDuration),
                new MySqlParameter("@expireTime", characterItem.expireTime),
                new MySqlParameter("@randomSeed", characterItem.randomSeed),
                new MySqlParameter("@ammo", characterItem.ammo),
                new MySqlParameter("@sockets", characterItem.WriteSockets()),
                new MySqlParameter("@version", characterItem.version));
        }

        private bool GetStorageItem(MySqlDataReader reader, out CharacterItem result)
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

        public override async UniTask<List<CharacterItem>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            List<CharacterItem> result = new List<CharacterItem>();
            await ExecuteReader((reader) =>
            {
                CharacterItem tempInventory;
                while (GetStorageItem(reader, out tempInventory))
                {
                    result.Add(tempInventory);
                }
            }, "SELECT id, dataId, level, amount, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version FROM storageitem WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId ORDER BY idx ASC",
                new MySqlParameter("@storageType", (byte)storageType),
                new MySqlParameter("@storageOwnerId", storageOwnerId));
            return result;
        }

        public override async UniTask UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await FillStorageItems(connection, transaction, storageType, storageOwnerId, storageItems);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public override async UniTask UpdateStorageAndCharacterItems(
            StorageType storageType,
            string storageOwnerId,
            List<CharacterItem> storageItems,
            string characterId,
            List<EquipWeapons> selectableWeaponSets,
            List<CharacterItem> equipItems,
            List<CharacterItem> nonEquipItems)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await FillStorageItems(connection, transaction, storageType, storageOwnerId, storageItems);
                        await FillCharacterItems(connection, transaction, characterId, selectableWeaponSets, equipItems, nonEquipItems);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private async UniTask FillStorageItems(MySqlConnection connection, MySqlTransaction transaction, StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            try
            {
                await DeleteStorageItems(connection, transaction, storageType, storageOwnerId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < storageItems.Count; ++i)
                {
                    await CreateStorageItem(connection, transaction, insertedIds, i, storageType, storageOwnerId, storageItems[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing storage items");
                LogException(LogTag, ex);
                throw;
            }
        }

        public async UniTask DeleteStorageItems(MySqlConnection connection, MySqlTransaction transaction, StorageType storageType, string storageOwnerId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM storageitem WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                new MySqlParameter("@storageType", (byte)storageType),
                new MySqlParameter("@storageOwnerId", storageOwnerId));
        }

        public override async UniTask<long> FindReservedStorage(StorageType storageType, string storageOwnerId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM storage_reservation WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                new MySqlParameter("@storageType", (byte)storageType),
                new MySqlParameter("@storageOwnerId", storageOwnerId));
            return result != null ? (long)result : 0;
        }

        public override async UniTask UpdateReservedStorage(StorageType storageType, string storageOwnerId, string reserverId)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await ExecuteNonQuery(connection, transaction, "DELETE FROM storage_reservation WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                            new MySqlParameter("@storageType", (byte)storageType),
                            new MySqlParameter("@storageOwnerId", storageOwnerId));
                        await ExecuteNonQuery(connection, transaction, "INSERT INTO storage_reservation (storageType, storageOwnerId, reserverId) VALUES (@storageType, @storageOwnerId, @reserverId)",
                            new MySqlParameter("@storageType", (byte)storageType),
                            new MySqlParameter("@storageOwnerId", storageOwnerId),
                            new MySqlParameter("@reserverId", reserverId));
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception ex)
                    {
                        LogError(LogTag, "Transaction, Error occurs while replacing storage reserving");
                        LogException(LogTag, ex);
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        public override async UniTask DeleteReservedStorage(StorageType storageType, string storageOwnerId)
        {
            await ExecuteNonQuery("DELETE FROM storage_reservation WHERE storageType=@storageType AND storageOwnerId=@storageOwnerId",
                new MySqlParameter("@storageType", (byte)storageType),
                new MySqlParameter("@storageOwnerId", storageOwnerId));
        }

        public override async UniTask DeleteReservedStorageByReserver(string reserverId)
        {
            await ExecuteNonQuery("DELETE FROM storage_reservation WHERE reserverId=@reserverId",
                new MySqlParameter("@reserverId", reserverId));
        }

        public override async UniTask DeleteAllReservedStorage()
        {
            await ExecuteNonQuery("DELETE FROM storage_reservation");
        }
    }
}
#endif