using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterHotkeySerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterHotkey data = (CharacterHotkey)obj;
            info.AddValue("hotkeyId", data.hotkeyId);
            info.AddValue("type", (byte)data.type);
            info.AddValue("relateId", data.relateId);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterHotkey data = (CharacterHotkey)obj;
            data.hotkeyId = info.GetString("hotkeyId");
            data.type = (HotkeyType)info.GetByte("type");
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.relateId = info.GetString("relateId");
            }
            catch { }
            obj = data;
            return obj;
        }
    }
}
