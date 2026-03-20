#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_GET_STORAGE_ITEMS_USERS = "GET_STORAGE_ITEMS_USERS";
        public const string CACHE_KEY_GET_STORAGE_ITEMS_GUILDS = "GET_STORAGE_ITEMS_GUILDS";
        public const string CACHE_KEY_GET_STORAGE_ITEMS_BUILDINGS = "GET_STORAGE_ITEMS_BUILDINGS";
        public override async UniTask<List<CharacterItem>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            switch (storageType)
            {
                case StorageType.Player:
                    return await PostgreSQLHelpers.ExecuteSelectJson<List<CharacterItem>>(connection, "storage_users", storageOwnerId);
                case StorageType.Guild:
                    return await PostgreSQLHelpers.ExecuteSelectJson<List<CharacterItem>>(connection, "storage_guilds", int.Parse(storageOwnerId));
                case StorageType.Building:
                    return await PostgreSQLHelpers.ExecuteSelectJson<List<CharacterItem>>(connection, "storage_buildings", storageOwnerId);
            }
            return new List<CharacterItem>();
        }

        public override async UniTask UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> characterItems)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await UpdateStorageItems(connection, null, storageType, storageOwnerId, characterItems);
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
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await UpdateStorageItems(connection, transaction, storageType, storageOwnerId, storageItems);
                if (selectableWeaponSets != null)
                    await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_selectable_weapon_sets", characterId, selectableWeaponSets);
                if (equipItems != null)
                    await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_equip_items", characterId, equipItems);
                if (nonEquipItems != null)
                    await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_non_equip_items", characterId, nonEquipItems);
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, $"Transaction, Error occurs while update storage and character items: {storageType}, {storageOwnerId}");
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async UniTask UpdateStorageItems(NpgsqlConnection connection, NpgsqlTransaction transaction, StorageType storageType, string storageOwnerId, List<CharacterItem> characterItems)
        {
            switch (storageType)
            {
                case StorageType.Player:
                    await PostgreSQLHelpers.ExecuteUpsertJson(
                        connection, transaction,
                        "storage_users",
                        storageOwnerId,
                        characterItems);
                    break;
                case StorageType.Guild:
                    await PostgreSQLHelpers.ExecuteUpsertJson(
                        connection, transaction,
                        "storage_guilds",
                        int.Parse(storageOwnerId),
                        characterItems);
                    break;
                case StorageType.Building:
                    await PostgreSQLHelpers.ExecuteUpsertJson(
                        connection, transaction,
                        "storage_buildings",
                        storageOwnerId,
                        characterItems);
                    break;
            }
        }

        public const string CACHE_KEY_FIND_RESERVED_STORAGE_USERS = "FIND_RESERVED_STORAGE_USERS";
        public const string CACHE_KEY_FIND_RESERVED_STORAGE_GUILDS = "FIND_RESERVED_STORAGE_GUILDS";
        public const string CACHE_KEY_FIND_RESERVED_STORAGE_BUILDINGS = "FIND_RESERVED_STORAGE_BUILDINGS";
        public override async UniTask<long> FindReservedStorage(StorageType storageType, string storageOwnerId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            switch (storageType)
            {
                case StorageType.Player:
                    return await PostgreSQLHelpers.ExecuteCount(
                        CACHE_KEY_FIND_RESERVED_STORAGE_USERS,
                        connection,
                        "storage_reservation_users",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                case StorageType.Guild:
                    return await PostgreSQLHelpers.ExecuteCount(
                        CACHE_KEY_FIND_RESERVED_STORAGE_GUILDS,
                        connection,
                        "storage_reservation_guilds",
                        PostgreSQLHelpers.WhereEqualTo("id", int.Parse(storageOwnerId)));
                case StorageType.Building:
                    return await PostgreSQLHelpers.ExecuteCount(
                        CACHE_KEY_FIND_RESERVED_STORAGE_BUILDINGS,
                        connection,
                        "storage_reservation_buildings",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
            }
            return 0;
        }

        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_USERS = "UPDATE_RESERVED_STORAGE_USERS";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_GUILDS = "UPDATE_RESERVED_STORAGE_GUILDS";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_BUILDINGS = "UPDATE_RESERVED_STORAGE_BUILDINGS";
        public override async UniTask UpdateReservedStorage(StorageType storageType, string storageOwnerId, string reserverId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            switch (storageType)
            {
                case StorageType.Player:
                    await PostgreSQLHelpers.ExecuteUpsert(
                        CACHE_KEY_UPDATE_RESERVED_STORAGE_USERS,
                        connection, null,
                        "storage_reservation_users",
                        "id",
                        new PostgreSQLHelpers.ColumnInfo("id", storageOwnerId),
                        new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId));
                    break;
                case StorageType.Guild:
                    await PostgreSQLHelpers.ExecuteUpsert(
                        CACHE_KEY_UPDATE_RESERVED_STORAGE_GUILDS,
                        connection, null,
                        "storage_reservation_guilds",
                        "id",
                        new PostgreSQLHelpers.ColumnInfo("id", int.Parse(storageOwnerId)),
                        new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId));
                    break;
                case StorageType.Building:
                    await PostgreSQLHelpers.ExecuteUpsert(
                        CACHE_KEY_UPDATE_RESERVED_STORAGE_BUILDINGS,
                        connection, null,
                        "storage_reservation_buildings",
                        "id",
                        new PostgreSQLHelpers.ColumnInfo("id", storageOwnerId),
                        new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId));
                    break;
            }
        }

        public override async UniTask DeleteReservedStorage(StorageType storageType, string storageOwnerId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await DeleteReservedStorage(connection, null, storageType, storageOwnerId);
        }

        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_USERS = "DELETE_RESERVED_STORAGE_USERS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_GUILDS = "DELETE_RESERVED_STORAGE_GUILDS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BUILDINGS = "DELETE_RESERVED_STORAGE_BUILDINGS";
        public async UniTask DeleteReservedStorage(NpgsqlConnection connection, NpgsqlTransaction transaction, StorageType storageType, string storageOwnerId)
        {
            switch (storageType)
            {
                case StorageType.Player:
                    await PostgreSQLHelpers.ExecuteDelete(
                        CACHE_KEY_DELETE_RESERVED_STORAGE_USERS,
                        connection, transaction,
                        "storage_reservation_users",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                    break;
                case StorageType.Guild:
                    await PostgreSQLHelpers.ExecuteDelete(
                        CACHE_KEY_DELETE_RESERVED_STORAGE_GUILDS,
                        connection, transaction,
                        "storage_reservation_guilds",
                        PostgreSQLHelpers.WhereEqualTo("id", int.Parse(storageOwnerId)));
                    break;
                case StorageType.Building:
                    await PostgreSQLHelpers.ExecuteDelete(
                        CACHE_KEY_DELETE_RESERVED_STORAGE_BUILDINGS,
                        connection, transaction,
                        "storage_reservation_buildings",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                    break;
            }
        }

        public async override UniTask DeleteReservedStorageByReserver(string reserverId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            await DeleteReservedStorageByReserver(connection, transaction, reserverId);
            await transaction.CommitAsync();
        }

        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_USERS = "DELETE_RESERVED_STORAGE_BY_RESERVER_USERS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_GUILDS = "DELETE_RESERVED_STORAGE_BY_RESERVER_GUILDS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_BUILDINGS = "DELETE_RESERVED_STORAGE_BY_RESERVER_BUILDINGS";
        public async UniTask DeleteReservedStorageByReserver(NpgsqlConnection connection, NpgsqlTransaction transaction, string reserverId)
        {
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_USERS,
                connection, transaction,
                "storage_reservation_users",
                PostgreSQLHelpers.WhereEqualTo("reserver_id", reserverId));
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_GUILDS,
                connection, transaction,
                "storage_reservation_guilds",
                PostgreSQLHelpers.WhereEqualTo("reserver_id", reserverId));
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_BUILDINGS,
                connection, transaction,
                "storage_reservation_buildings",
                PostgreSQLHelpers.WhereEqualTo("reserver_id", reserverId));
        }

        public override async UniTask DeleteAllReservedStorage()
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            using var cmd = new NpgsqlCommand("DELETE FROM storage_reservation_users", connection, transaction);
            await cmd.PrepareAsync();
            await cmd.ExecuteNonQueryAsync();
            using var cmd2 = new NpgsqlCommand("DELETE FROM storage_reservation_guilds", connection, transaction);
            await cmd2.PrepareAsync();
            await cmd2.ExecuteNonQueryAsync();
            using var cmd3 = new NpgsqlCommand("DELETE FROM storage_reservation_buildings", connection, transaction);
            await cmd3.PrepareAsync();
            await cmd3.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
    }
}
#endif