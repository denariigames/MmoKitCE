using Insthync.DevExtension;
using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public static class BuildingSaveDataExtensions
    {
        private static System.Type classType;
        public static System.Type ClassType
        {
            get
            {
                if (classType == null)
                    classType = typeof(BuildingSaveDataExtensions);
                return classType;
            }
        }

        public static T CloneTo<T>(this IBuildingSaveData from, T to) where T : IBuildingSaveData
        {
            to.Id = from.Id;
            to.ParentId = from.ParentId;
            to.EntityId = from.EntityId;
            to.CurrentHp = from.CurrentHp;
            to.RemainsLifeTime = from.RemainsLifeTime;
            to.IsLocked = from.IsLocked;
            to.LockPassword = from.LockPassword;
            to.Position = from.Position;
            to.Rotation = from.Rotation;
            to.CreatorId = from.CreatorId;
            to.CreatorName = from.CreatorName;
            to.ExtraData = from.ExtraData;
            to.IsSceneObject = from.IsSceneObject;
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "CloneTo", from, to);
            return to;
        }

        public static void SerializeBuildingSaveData<T>(this T buildingSaveData, NetDataWriter writer) where T : IBuildingSaveData
        {
            writer.Put(buildingSaveData.Id);
            writer.Put(buildingSaveData.ParentId);
            writer.Put(buildingSaveData.EntityId);
            writer.Put(buildingSaveData.CurrentHp);
            writer.Put(buildingSaveData.RemainsLifeTime);
            writer.Put(buildingSaveData.IsLocked);
            writer.Put(buildingSaveData.LockPassword);
            writer.Put(buildingSaveData.Position);
            writer.Put(buildingSaveData.Rotation);
            writer.Put(buildingSaveData.CreatorId);
            writer.Put(buildingSaveData.CreatorName);
            writer.Put(buildingSaveData.ExtraData);
            writer.Put(buildingSaveData.IsSceneObject);
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "SerializeBuildingSaveData", buildingSaveData, writer);
        }

        public static BuildingSaveData DeserializeBuildingSaveData(this NetDataReader reader)
        {
            return new BuildingSaveData().DeserializeBuildingSaveData(reader);
        }

        public static void DeserializeBuildingSaveData(this NetDataReader reader, ref BuildingSaveData buildingSaveData)
        {
            buildingSaveData = reader.DeserializeBuildingSaveData();
        }

        public static T DeserializeBuildingSaveData<T>(this T buildingSaveData, NetDataReader reader) where T : IBuildingSaveData
        {
            buildingSaveData.Id = reader.GetString();
            buildingSaveData.ParentId = reader.GetString();
            buildingSaveData.EntityId = reader.GetInt();
            buildingSaveData.CurrentHp = reader.GetInt();
            buildingSaveData.RemainsLifeTime = reader.GetFloat();
            buildingSaveData.IsLocked = reader.GetBool();
            buildingSaveData.LockPassword = reader.GetString();
            buildingSaveData.Position = reader.Get<Vec3>();
            buildingSaveData.Rotation = reader.Get<Vec3>();
            buildingSaveData.CreatorId = reader.GetString();
            buildingSaveData.CreatorName = reader.GetString();
            buildingSaveData.ExtraData = reader.GetString();
            buildingSaveData.IsSceneObject = reader.GetBool();
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "DeserializeBuildingSaveData", buildingSaveData, reader);
            return buildingSaveData;
        }
    }
}
