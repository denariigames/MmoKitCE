using Cysharp.Threading.Tasks;
using System.Data.Common;

namespace MultiplayerARPG.MMO
{
    public delegate UniTask<PlayerCharacterData> DbGetCharacterDelegate(
        PlayerCharacterData result,
        bool withEquipWeapons,
        bool withAttributes,
        bool withSkills,
        bool withSkillUsages,
        bool withBuffs,
        bool withEquipItems,
        bool withNonEquipItems,
        bool withSummons,
        bool withHotkeys,
        bool withQuests,
        bool withCurrencies,
        bool withServerCustomData,
        bool withPrivateCustomData,
        bool withPublicCustomData);

    public delegate UniTask DbCreateCharacterDelegate<TConnection, TTransaction>(
        TConnection connection,
        TTransaction transaction,
        string userId,
        IPlayerCharacterData createData)
        where TConnection : DbConnection
        where TTransaction : DbTransaction;

    public delegate UniTask DbUpdateCharacterDelegate<TConnection, TTransaction>(
        TConnection connection,
        TTransaction transaction,
        TransactionUpdateCharacterState state,
        IPlayerCharacterData updateData)
        where TConnection : DbConnection
        where TTransaction : DbTransaction;

    public delegate UniTask DbDeleteCharacterDelegate<TConnection, TTransaction>(
        TConnection connection,
        TTransaction transaction,
        string userId,
        string characterId)
        where TConnection : DbConnection
        where TTransaction : DbTransaction;
}
