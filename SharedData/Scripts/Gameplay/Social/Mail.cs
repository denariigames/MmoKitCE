using System.Collections.Generic;
using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class Mail : INetSerializable
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Gold { get; set; }
        public int Cash { get; set; }
        public List<CharacterCurrency> Currencies { get; } = new List<CharacterCurrency>();
        public List<CharacterItem> Items { get; } = new List<CharacterItem>();
        public List<UnlockableContent> UnlockableContents { get; } = new List<UnlockableContent>();
        public bool IsRead { get; set; }
        public long ReadTimestamp { get; set; }
        public bool IsClaim { get; set; }
        public long ClaimTimestamp { get; set; }
        public bool IsDelete { get; set; }
        public long DeleteTimestamp { get; set; }
        public long SentTimestamp { get; set; }

        public bool HaveItemsToClaim()
        {
            return Gold != 0 || Cash != 0 ||
                Currencies.Count > 0 || Items.Count > 0 ||
                UnlockableContents.Count > 0;
        }

        public List<CharacterCurrency> ReadCurrencies(string currenciesString)
        {
            Currencies.Clear();
            Currencies.AddRange(currenciesString.ReadCurrencies());
            return Currencies;
        }

        public string WriteCurrencies()
        {
            return Currencies.WriteCurrencies();
        }

        public List<UnlockableContent> ReadUnlockableContents(string unlockableContentsString)
        {
            UnlockableContents.Clear();
            UnlockableContents.AddRange(unlockableContentsString.ReadUnlockableContents());
            return UnlockableContents;
        }

        public string WriteUnlockableContents()
        {
            return UnlockableContents.WriteUnlockableContents();
        }

        public List<CharacterItem> ReadItems(string itemsString)
        {
            Items.Clear();
            Items.AddRange(itemsString.ReadItems());
            return Items;
        }

        public string WriteItems()
        {
            return Items.WriteItems();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(EventId);
            writer.Put(SenderId);
            writer.Put(SenderName);
            writer.Put(ReceiverId);
            writer.Put(Title);
            writer.Put(Content);
            writer.PutPackedInt(Gold);
            writer.PutPackedInt(Cash);
            writer.Put(WriteCurrencies());
            writer.Put(WriteItems());
            writer.Put(WriteUnlockableContents());
            writer.Put(IsRead);
            writer.PutPackedLong(ReadTimestamp);
            writer.Put(IsClaim);
            writer.PutPackedLong(ClaimTimestamp);
            writer.Put(IsDelete);
            writer.PutPackedLong(DeleteTimestamp);
            writer.PutPackedLong(SentTimestamp);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
            EventId = reader.GetString();
            SenderId = reader.GetString();
            SenderName = reader.GetString();
            ReceiverId = reader.GetString();
            Title = reader.GetString();
            Content = reader.GetString();
            Gold = reader.GetPackedInt();
            Cash = reader.GetPackedInt();
            ReadCurrencies(reader.GetString());
            ReadItems(reader.GetString());
            ReadUnlockableContents(reader.GetString());
            IsRead = reader.GetBool();
            ReadTimestamp = reader.GetPackedLong();
            IsClaim = reader.GetBool();
            ClaimTimestamp = reader.GetPackedLong();
            IsDelete = reader.GetBool();
            DeleteTimestamp = reader.GetPackedLong();
            SentTimestamp = reader.GetPackedLong();
        }
    }
}
