#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool GetBuilding(MySqlDataReader reader, out BuildingSaveData result)
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
                result.LockPassword = reader.GetString(6);
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

        public override async UniTask CreateBuilding(string channel, string mapName, IBuildingSaveData building)
        {
            string extraData = building.ExtraData;
            if (string.IsNullOrEmpty(extraData))
                extraData = "";
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                await ExecuteNonQuery(connection, null, "INSERT INTO buildings (id, channel, parentId, entityId, currentHp, remainsLifeTime, mapName, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, creatorId, creatorName, extraData, isSceneObject) VALUES (@id, @channel, @parentId, @entityId, @currentHp, @remainsLifeTime, @mapName, @positionX, @positionY, @positionZ, @rotationX, @rotationY, @rotationZ, @creatorId, @creatorName, @extraData, @isSceneObject)",
                    new MySqlParameter("@id", building.Id),
                    new MySqlParameter("@channel", channel),
                    new MySqlParameter("@parentId", building.ParentId),
                    new MySqlParameter("@entityId", building.EntityId),
                    new MySqlParameter("@currentHp", building.CurrentHp),
                    new MySqlParameter("@remainsLifeTime", building.RemainsLifeTime),
                    new MySqlParameter("@mapName", mapName),
                    new MySqlParameter("@positionX", building.Position.x),
                    new MySqlParameter("@positionY", building.Position.y),
                    new MySqlParameter("@positionZ", building.Position.z),
                    new MySqlParameter("@rotationX", building.Rotation.x),
                    new MySqlParameter("@rotationY", building.Rotation.y),
                    new MySqlParameter("@rotationZ", building.Rotation.z),
                    new MySqlParameter("@creatorId", building.CreatorId),
                    new MySqlParameter("@creatorName", building.CreatorName),
                    new MySqlParameter("@extraData", extraData),
                    new MySqlParameter("@isSceneObject", building.IsSceneObject));
            }
        }

        public override async UniTask<List<BuildingSaveData>> GetBuildings(string channel, string mapName)
        {
            List<BuildingSaveData> result = new List<BuildingSaveData>();
            await ExecuteReader((reader) =>
            {
                BuildingSaveData tempBuilding;
                while (GetBuilding(reader, out tempBuilding))
                {
                    result.Add(tempBuilding);
                }
            }, "SELECT id, parentId, entityId, currentHp, remainsLifeTime, isLocked, lockPassword, creatorId, creatorName, extraData, isSceneObject, positionX, positionY, positionZ, rotationX, rotationY, rotationZ FROM buildings WHERE channel=@channel AND mapName=@mapName",
                new MySqlParameter("@channel", channel),
                new MySqlParameter("@mapName", mapName));
            return result;
        }

        public override async UniTask UpdateBuilding(string channel, string mapName, IBuildingSaveData building)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await ExecuteNonQuery(connection, transaction, "UPDATE buildings SET " +
                            "parentId=@parentId, " +
                            "entityId=@entityId, " +
                            "currentHp=@currentHp, " +
                            "remainsLifeTime=@remainsLifeTime, " +
                            "isLocked=@isLocked, " +
                            "lockPassword=@lockPassword, " +
                            "creatorId=@creatorId, " +
                            "creatorName=@creatorName, " +
                            "extraData=@extraData, " +
                            "isSceneObject=@isSceneObject, " +
                            "positionX=@positionX, " +
                            "positionY=@positionY, " +
                            "positionZ=@positionZ, " +
                            "rotationX=@rotationX, " +
                            "rotationY=@rotationY, " +
                            "rotationZ=@rotationZ " +
                            "WHERE id=@id AND channel=@channel AND mapName=@mapName",
                            new MySqlParameter("@id", building.Id),
                            new MySqlParameter("@parentId", building.ParentId),
                            new MySqlParameter("@entityId", building.EntityId),
                            new MySqlParameter("@currentHp", building.CurrentHp),
                            new MySqlParameter("@remainsLifeTime", building.RemainsLifeTime),
                            new MySqlParameter("@isLocked", building.IsLocked),
                            new MySqlParameter("@lockPassword", building.LockPassword),
                            new MySqlParameter("@creatorId", building.CreatorId),
                            new MySqlParameter("@creatorName", building.CreatorName),
                            new MySqlParameter("@extraData", building.ExtraData),
                            new MySqlParameter("@isSceneObject", building.IsSceneObject),
                            new MySqlParameter("@positionX", building.Position.x),
                            new MySqlParameter("@positionY", building.Position.y),
                            new MySqlParameter("@positionZ", building.Position.z),
                            new MySqlParameter("@rotationX", building.Rotation.x),
                            new MySqlParameter("@rotationY", building.Rotation.y),
                            new MySqlParameter("@rotationZ", building.Rotation.z),
                            new MySqlParameter("@channel", channel),
                            new MySqlParameter("@mapName", mapName));

                        await transaction.CommitAsync();
                    }
                    catch (System.Exception ex)
                    {
                        LogError(LogTag, "Transaction, Error occurs while update building: " + building.Id);
                        LogException(LogTag, ex);
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        public override async UniTask DeleteBuilding(string channel, string mapName, string id)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                await ExecuteNonQuery(connection, null, "DELETE FROM buildings WHERE id=@id AND channel=@channel AND mapName=@mapName",
                    new MySqlParameter("@id", id),
                    new MySqlParameter("@channel", channel),
                    new MySqlParameter("@mapName", mapName));
            }
        }
    }
}
#endif