using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateCharacterReq : INetSerializable
    {
        public TransactionUpdateCharacterState State { get; set; }
        public PlayerCharacterData CharacterData { get; set; }
        public List<CharacterBuff>? SummonBuffs { get; set; }
        public bool DeleteStorageReservation { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            State = (TransactionUpdateCharacterState)reader.GetPackedUInt();
            CharacterData = reader.Get(() => new PlayerCharacterData());
            SummonBuffs = reader.GetList<CharacterBuff>();
            DeleteStorageReservation = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUInt((uint)State);
            writer.Put(CharacterData);
            writer.PutList(SummonBuffs);
            writer.Put(DeleteStorageReservation);
        }
    }
}