using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public delegate CharacterItem CharacterItemCloneDelegate(
        ref CharacterItem src,
        CharacterItem dst);

    public delegate void CharacterItemSerializeDelegate(
        ref CharacterItem characterItem,
        NetDataWriter writer);

    public delegate void CharacterItemDeserializeDelegate(
        ref CharacterItem characterItem,
        NetDataReader reader);
}
