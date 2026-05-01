using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct GuildListEntry : INetSerializable
    {
        public int Id { get; set; }
        public string GuildName { get; set; }
        public int Level { get; set; }
        public GuildListFieldOptions FieldOptions { get; set; }
        public string GuildMessage { get; set; }
        public string GuildMessage2 { get; set; }
        public int Score { get; set; }
        public string Options { get; set; }
        public bool AutoAcceptRequests { get; set; }
        public int Rank { get; set; }
        public int CurrentMembers { get; set; }
        public int MaxMembers { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(Id);
            writer.Put(GuildName);
            writer.PutPackedInt(Level);
            writer.PutPackedInt((int)FieldOptions);
            if (FieldOptions.Has(GuildListFieldOptions.GuildMessage))
                writer.Put(GuildMessage);
            if (FieldOptions.Has(GuildListFieldOptions.GuildMessage2))
                writer.Put(GuildMessage2);
            if (FieldOptions.Has(GuildListFieldOptions.Score))
                writer.PutPackedInt(Score);
            if (FieldOptions.Has(GuildListFieldOptions.Options))
                writer.Put(Options);
            if (FieldOptions.Has(GuildListFieldOptions.AutoAcceptRequests))
                writer.Put(AutoAcceptRequests);
            if (FieldOptions.Has(GuildListFieldOptions.Rank))
                writer.PutPackedInt(Rank);
            if (FieldOptions.Has(GuildListFieldOptions.CurrentMembers))
                writer.PutPackedInt(CurrentMembers);
            if (FieldOptions.Has(GuildListFieldOptions.MaxMembers))
                writer.PutPackedInt(MaxMembers);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetPackedInt();
            GuildName = reader.GetString();
            Level = reader.GetPackedInt();
            FieldOptions = (GuildListFieldOptions)reader.GetPackedInt();
            if (FieldOptions.Has(GuildListFieldOptions.GuildMessage))
                GuildMessage = reader.GetString();
            if (FieldOptions.Has(GuildListFieldOptions.GuildMessage2))
                GuildMessage2 = reader.GetString();
            if (FieldOptions.Has(GuildListFieldOptions.Score))
                Score = reader.GetPackedInt();
            if (FieldOptions.Has(GuildListFieldOptions.Options))
                Options = reader.GetString();
            if (FieldOptions.Has(GuildListFieldOptions.AutoAcceptRequests))
                AutoAcceptRequests = reader.GetBool();
            if (FieldOptions.Has(GuildListFieldOptions.Rank))
                Rank = reader.GetPackedInt();
            if (FieldOptions.Has(GuildListFieldOptions.CurrentMembers))
                CurrentMembers = reader.GetPackedInt();
            if (FieldOptions.Has(GuildListFieldOptions.MaxMembers))
                MaxMembers = reader.GetPackedInt();
        }
    }
}
