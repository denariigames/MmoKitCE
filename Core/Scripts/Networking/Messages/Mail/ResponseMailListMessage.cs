using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct ResponseMailListMessage : INetSerializable
    {
        public bool onlyNewMails;
        public List<MailListEntry> mails;

        public void Deserialize(NetDataReader reader)
        {
            onlyNewMails = reader.GetBool();
            mails = reader.GetList<MailListEntry>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(onlyNewMails);
            writer.PutList(mails);
        }
    }
}
