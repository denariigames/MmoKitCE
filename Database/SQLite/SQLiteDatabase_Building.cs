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
        private bool GetBuilding(SqliteDataReader reader, out BuildingSaveData result)
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

        public override UniTask CreateBuilding(string channel, string mapName, IBuildingSaveData building)
        {
            string extraData = building.ExtraData;
            if (string.IsNullOrEmpty(extraData))
                extraData = "";
            ExecuteNonQuery("INSERT INTO buildings (id, channel, parentId, entityId, currentHp, remainsLifeTime, mapName, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, creatorId, creatorName, extraData, isSceneObject) VALUES (@id, @channel, @parentId, @entityId, @currentHp, @remainsLifeTime, @mapName, @positionX, @positionY, @positionZ, @rotationX, @rotationY, @rotationZ, @creatorId, @creatorName, @extraData, @isSceneObject)",
                new SqliteParameter("@id", building.Id),
                new SqliteParameter("@channel", channel),
                new SqliteParameter("@parentId", building.ParentId),
                new SqliteParameter("@entityId", building.EntityId),
                new SqliteParameter("@currentHp", building.CurrentHp),
                new SqliteParameter("@remainsLifeTime", building.RemainsLifeTime),
                new SqliteParameter("@mapName", mapName),
                new SqliteParameter("@positionX", building.Position.x),
                new SqliteParameter("@positionY", building.Position.y),
                new SqliteParameter("@positionZ", building.Position.z),
                new SqliteParameter("@rotationX", building.Rotation.x),
                new SqliteParameter("@rotationY", building.Rotation.y),
                new SqliteParameter("@rotationZ", building.Rotation.z),
                new SqliteParameter("@creatorId", building.CreatorId),
                new SqliteParameter("@creatorName", building.CreatorName),
                new SqliteParameter("@extraData", extraData),
                new SqliteParameter("@isSceneObject", building.IsSceneObject));
            return new UniTask();
        }

        public override UniTask<List<BuildingSaveData>> GetBuildings(string channel, string mapName)
        {
            List<BuildingSaveData> result = new List<BuildingSaveData>();
            ExecuteReader((reader) =>
            {
                BuildingSaveData tempBuilding;
                while (GetBuilding(reader, out tempBuilding))
                {
                    result.Add(tempBuilding);
                }
            }, "SELECT id, parentId, entityId, currentHp, remainsLifeTime, isLocked, lockPassword, creatorId, creatorName, extraData, isSceneObject, positionX, positionY, positionZ, rotationX, rotationY, rotationZ FROM buildings WHERE channel=@channel AND mapName=@mapName",
                new SqliteParameter("@channel", channel),
                new SqliteParameter("@mapName", mapName));
            return new UniTask<List<BuildingSaveData>>(result);
        }

        public override UniTask UpdateBuilding(string channel, string mapName, IBuildingSaveData building)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteNonQuery(transaction, "UPDATE buildings SET " +
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
                    new SqliteParameter("@id", building.Id),
                    new SqliteParameter("@parentId", building.ParentId),
                    new SqliteParameter("@entityId", building.EntityId),
                    new SqliteParameter("@currentHp", building.CurrentHp),
                    new SqliteParameter("@remainsLifeTime", building.RemainsLifeTime),
                    new SqliteParameter("@isLocked", building.IsLocked),
                    new SqliteParameter("@lockPassword", building.LockPassword),
                    new SqliteParameter("@creatorId", building.CreatorId),
                    new SqliteParameter("@creatorName", building.CreatorName),
                    new SqliteParameter("@extraData", building.ExtraData),
                    new SqliteParameter("@isSceneObject", building.IsSceneObject),
                    new SqliteParameter("@positionX", building.Position.x),
                    new SqliteParameter("@positionY", building.Position.y),
                    new SqliteParameter("@positionZ", building.Position.z),
                    new SqliteParameter("@rotationX", building.Rotation.x),
                    new SqliteParameter("@rotationY", building.Rotation.y),
                    new SqliteParameter("@rotationZ", building.Rotation.z),
                    new SqliteParameter("@channel", channel),
                    new SqliteParameter("@mapName", mapName));

                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while update building: " + building.Id);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
            return new UniTask();
        }

        public override UniTask DeleteBuilding(string channel, string mapName, string id)
        {
            ExecuteNonQuery("DELETE FROM buildings WHERE id=@id AND channel=@channel AND mapName=@mapName",
                new SqliteParameter("@id", id),
                new SqliteParameter("@channel", channel),
                new SqliteParameter("@mapName", mapName));
            return new UniTask();
        }
    }
}
#endif