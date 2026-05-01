#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public override async UniTask<List<CharacterBuff>> GetSummonBuffs(string characterId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            return await PostgreSQLHelpers.ExecuteSelectJson<List<CharacterBuff>>(connection, "character_summon_buffs", characterId);
        }
    }
}
#endif
