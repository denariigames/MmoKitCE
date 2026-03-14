using System.Runtime.Serialization;
using UnityEngine;

namespace MultiplayerARPG
{
    public class BuildingSaveDataSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            BuildingSaveData data = (BuildingSaveData)obj;
            info.AddValue("id", data.Id);
            info.AddValue("parentId", data.ParentId);
            info.AddValue("entityId", data.EntityId);
            info.AddValue("currentHp", data.CurrentHp);
            info.AddValue("remainsLifeTime", data.RemainsLifeTime);
            info.AddValue("position", (Vector3)data.Position);
            info.AddValue("rotation", Quaternion.Euler(data.Rotation));
            info.AddValue("isLocked", data.IsLocked);
            info.AddValue("lockPassword", data.LockPassword);
            info.AddValue("creatorId", data.CreatorId);
            info.AddValue("creatorName", data.CreatorName);
            info.AddValue("extraData", data.ExtraData);
            info.AddValue("isSceneObject", data.IsSceneObject);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            BuildingSaveData data = (BuildingSaveData)obj;
            data.Id = info.GetString("id");
            data.ParentId = info.GetString("parentId");
            data.EntityId = info.GetInt32("entityId");
            data.CurrentHp = info.GetInt32("currentHp");
            data.RemainsLifeTime = info.GetSingle("remainsLifeTime");
            data.Position = (Vector3)info.GetValue("position", typeof(Vector3));
            data.Rotation = ((Quaternion)info.GetValue("rotation", typeof(Quaternion))).eulerAngles;
            data.IsLocked = info.GetBoolean("isLocked");
            data.LockPassword = info.GetString("lockPassword");
            data.CreatorId = info.GetString("creatorId");
            data.CreatorName = info.GetString("creatorName");
            data.ExtraData = info.GetString("extraData");
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.IsSceneObject = info.GetBoolean("isSceneObject");
            }
            catch { }
            obj = data;
            return obj;
        }
    }
}
