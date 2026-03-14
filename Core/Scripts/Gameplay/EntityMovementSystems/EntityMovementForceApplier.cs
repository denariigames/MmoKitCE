using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace MultiplayerARPG
{
    public class EntityMovementForceApplier : INetSerializable
    {
        public ApplyMovementForceMode Mode { get; set; }
        public Vector3 Direction { get; set; }
        public ApplyMovementForceSourceType SourceType { get; set; }
        public int SourceDataId { get; set; }
        public int SourceLevel { get; set; }
        public float CurrentSpeed { get; set; }
        public float Deceleration { get; set; }
        public float Duration { get; set; }
        public float Elasped { get; set; }
        public Vector3 Velocity { get => Direction * CurrentSpeed; }

        public EntityMovementForceApplier Apply(ApplyMovementForceMode mode, Vector3 direction, ApplyMovementForceSourceType sourceType, int sourceDataId, int sourceLevel, EntityMovementForceApplierData data)
        {
            Mode = mode;
            Direction = direction;
            SourceType = sourceType;
            SourceDataId = sourceDataId;
            SourceLevel = sourceLevel;
            CurrentSpeed = data.speed;
            Deceleration = data.deceleration;
            Duration = data.duration;
            Elasped = 0f;
            return this;
        }

        public EntityMovementForceApplier Apply(ApplyMovementForceMode mode, Vector3 direction, ApplyMovementForceSourceType sourceType, int sourceDataId, int sourceLevel, float speed, float deceleration, float duration)
        {
            Mode = mode;
            Direction = direction;
            SourceType = sourceType;
            SourceDataId = sourceDataId;
            SourceLevel = sourceLevel;
            CurrentSpeed = speed;
            Deceleration = deceleration;
            Duration = duration;
            Elasped = 0f;
            return this;
        }

        public void Deserialize(NetDataReader reader)
        {
            Mode = (ApplyMovementForceMode)reader.GetByte();
            Direction = reader.GetVector3();
            SourceType = (ApplyMovementForceSourceType)reader.GetByte();
            if (SourceType != ApplyMovementForceSourceType.None)
            {
                SourceDataId = reader.GetPackedInt();
                SourceLevel = reader.GetPackedInt();
            }
            CurrentSpeed = Mathf.HalfToFloat(reader.GetPackedUShort());
            Deceleration = Mathf.HalfToFloat(reader.GetPackedUShort());
            Duration = Mathf.HalfToFloat(reader.GetPackedUShort());
            Elasped = Mathf.HalfToFloat(reader.GetPackedUShort());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Mode);
            writer.PutVector3(Direction);
            writer.Put((byte)SourceType);
            if (SourceType != ApplyMovementForceSourceType.None)
            {
                writer.PutPackedInt(SourceDataId);
                writer.PutPackedInt(SourceLevel);
            }
            writer.PutPackedUShort(Mathf.FloatToHalf(CurrentSpeed));
            writer.PutPackedUShort(Mathf.FloatToHalf(Deceleration));
            writer.PutPackedUShort(Mathf.FloatToHalf(Duration));
            writer.PutPackedUShort(Mathf.FloatToHalf(Elasped));
        }

        public bool Update(float deltaTime)
        {
            if (CurrentSpeed <= 0f)
                return false;
            CurrentSpeed -= deltaTime * Deceleration;
            Elasped += deltaTime;
            if (Duration > 0f && Elasped >= Duration)
            {
                Elasped = Duration;
                CurrentSpeed = 0f;
            }
            return CurrentSpeed > 0f;
        }

        public BaseGameData GetSourceData()
        {
            switch (SourceType)
            {
                case ApplyMovementForceSourceType.Skill:
                    if (GameInstance.Skills.TryGetValue(SourceDataId, out BaseSkill skill))
                        return skill;
                    return null;
            }
            return null;
        }
    }

    public static class EntityMovementForceApplierExtensions
    {
        public static void RemoveReplaceMovementForces(this IList<EntityMovementForceApplier> forceAppliers)
        {
            for (int i = forceAppliers.Count - 1; i >= 0; --i)
            {
                if (forceAppliers[i].Mode.IsReplaceMovement())
                    forceAppliers.RemoveAt(i);
            }
        }

        public static void UpdateForces(this IList<EntityMovementForceApplier> forceAppliers, float deltaTime, float characterMoveSpeed, out Vector3 forceMotion, out EntityMovementForceApplier replaceMovementForceApplier)
        {
            forceMotion = Vector3.zero;
            replaceMovementForceApplier = null;
            for (int i = forceAppliers.Count - 1; i >= 0; --i)
            {
                if (!forceAppliers[i].Update(deltaTime) || forceAppliers[i].CurrentSpeed < characterMoveSpeed)
                {
                    forceAppliers.RemoveAt(i);
                    continue;
                }
                if (!forceAppliers[i].Mode.IsReplaceMovement())
                    forceMotion += forceAppliers[i].Velocity;
                else
                    replaceMovementForceApplier = forceAppliers[i];
            }
        }

        public static EntityMovementForceApplier FindBySource(this IList<EntityMovementForceApplier> forceAppliers, ApplyMovementForceSourceType sourceType, int sourceDataId)
        {
            for (int i = 0; i < forceAppliers.Count; ++i)
            {
                if (forceAppliers[i].SourceType == sourceType &&
                    forceAppliers[i].SourceDataId == sourceDataId)
                    return forceAppliers[i];
            }
            return null;
        }
    }
}
