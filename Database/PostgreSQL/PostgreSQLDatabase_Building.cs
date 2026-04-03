#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool GetBuilding(NpgsqlDataReader reader, out BuildingSaveData result)
        {
            if (reader.Read())
            {
                result = new BuildingSaveData();
                result.Id = reader.GetString(0);
                result.ParentId = reader.GetString(1);
                result.EntityId = reader.GetInt32(2);
                result.CurrentHp = reader.GetInt32(3);
                result.RemainsLifeTime = reader.GetFloat(4);
                result.IsLocked = reader.GetBoolean(5);
                result.LockPassword = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                result.CreatorId = reader.GetString(7);
                result.CreatorName = reader.GetString(8);
                result.ExtraData = reader.GetString(9);
                result.IsSceneObject = reader.GetBoolean(10);
                result.Position = new Vec3(reader.GetFloat(11), reader.GetFloat(12), reader.GetFloat(13));
                result.Rotation = new Vec3(reader.GetFloat(14), reader.GetFloat(15), reader.GetFloat(16));
                return true;
            }
            result = new BuildingSaveData();
            return false;
        }

        public const string CACHE_KEY_GET_BUILDINGS = "GET_BUILDINGS";
        public override async UniTask<List<BuildingSaveData>> GetBuildings(string channel, string mapName)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_BUILDINGS,
                connection,
                "buildings", "id, parent_id, entity_id, current_hp, remains_lifetime, is_locked, lock_password, creator_id, creator_name, extra_data, is_scene_object, position_x, position_y, position_z, rotation_x, rotation_y, rotation_z",
                PostgreSQLHelpers.WhereEqualTo("channel", channel),
                PostgreSQLHelpers.AndWhereEqualTo("map_name", mapName));
            List<BuildingSaveData> result = new List<BuildingSaveData>();
            BuildingSaveData tempBuilding;
            while (GetBuilding(reader, out tempBuilding))
            {
                result.Add(tempBuilding);
            }
            return result;
        }

        public const string CACHE_KEY_CREATE_BUILDING = "CREATE_BUILDING";
        public override async UniTask CreateBuilding(string channel, string mapName, IBuildingSaveData building)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            string extraData = building.ExtraData;
            if (string.IsNullOrEmpty(extraData))
                extraData = "";
            await PostgreSQLHelpers.ExecuteInsert(
                CACHE_KEY_CREATE_BUILDING,
                connection, null,
                "buildings",
                new PostgreSQLHelpers.ColumnInfo("id", building.Id),
                new PostgreSQLHelpers.ColumnInfo("channel", channel),
                new PostgreSQLHelpers.ColumnInfo("parent_id", building.ParentId),
                new PostgreSQLHelpers.ColumnInfo("entity_id", building.EntityId),
                new PostgreSQLHelpers.ColumnInfo("current_hp", building.CurrentHp),
                new PostgreSQLHelpers.ColumnInfo("remains_lifetime", building.RemainsLifeTime),
                new PostgreSQLHelpers.ColumnInfo("map_name", mapName),
                new PostgreSQLHelpers.ColumnInfo("position_x", building.Position.x),
                new PostgreSQLHelpers.ColumnInfo("position_y", building.Position.y),
                new PostgreSQLHelpers.ColumnInfo("position_z", building.Position.z),
                new PostgreSQLHelpers.ColumnInfo("rotation_x", building.Rotation.x),
                new PostgreSQLHelpers.ColumnInfo("rotation_y", building.Rotation.y),
                new PostgreSQLHelpers.ColumnInfo("rotation_z", building.Rotation.z),
                new PostgreSQLHelpers.ColumnInfo("creator_id", building.CreatorId),
                new PostgreSQLHelpers.ColumnInfo("creator_name", building.CreatorName),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Text, "extra_data", extraData),
                new PostgreSQLHelpers.ColumnInfo("is_scene_object", building.IsSceneObject));
        }

        public const string CACHE_KEY_UPDATE_BUILDING = "UPDATE_BUILDING";
        public override async UniTask UpdateBuilding(string channel, string mapName, IBuildingSaveData building)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteUpdate(
                    CACHE_KEY_UPDATE_BUILDING,
                    connection, transaction,
                    "buildings",
                    new[]
                    {
                        new PostgreSQLHelpers.ColumnInfo("parent_id", building.ParentId),
                        new PostgreSQLHelpers.ColumnInfo("entity_id", building.EntityId),
                        new PostgreSQLHelpers.ColumnInfo("current_hp", building.Id),
                        new PostgreSQLHelpers.ColumnInfo("remains_lifetime", building.RemainsLifeTime),
                        new PostgreSQLHelpers.ColumnInfo("is_locked", building.IsLocked),
                        new PostgreSQLHelpers.ColumnInfo("lock_password", building.LockPassword),
                        new PostgreSQLHelpers.ColumnInfo("position_x", building.Position.x),
                        new PostgreSQLHelpers.ColumnInfo("position_y", building.Position.y),
                        new PostgreSQLHelpers.ColumnInfo("position_z", building.Position.z),
                        new PostgreSQLHelpers.ColumnInfo("rotation_x", building.Rotation.x),
                        new PostgreSQLHelpers.ColumnInfo("rotation_y", building.Rotation.y),
                        new PostgreSQLHelpers.ColumnInfo("rotation_z", building.Rotation.z),
                        new PostgreSQLHelpers.ColumnInfo("creator_id", building.CreatorId),
                        new PostgreSQLHelpers.ColumnInfo("creator_name", building.CreatorName),
                        new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Text, "extra_data", building.ExtraData),
                        new PostgreSQLHelpers.ColumnInfo("is_scene_object", building.IsSceneObject),
                    },
                    new[]
                    {
                        PostgreSQLHelpers.WhereEqualTo("id", building.Id),
                        PostgreSQLHelpers.AndWhereEqualTo("channel", channel),
                        PostgreSQLHelpers.AndWhereEqualTo("map_name", mapName),
                    });

                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while update building: " + building.Id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public const string CACHE_KEY_DELETE_BUILDING = "DELETE_BUILDING";
        public override async UniTask DeleteBuilding(string channel, string mapName, string id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_BUILDING,
                connection, null,
                "buildings",
                PostgreSQLHelpers.WhereEqualTo("id", id),
                PostgreSQLHelpers.AndWhereEqualTo("channel", channel),
                PostgreSQLHelpers.AndWhereEqualTo("map_name", mapName));
        }
    }
}
#endif