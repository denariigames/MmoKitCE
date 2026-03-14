using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG
{
    public partial class GoldDropEntity : BaseRewardDropEntity, IPickupActivatableEntity
    {
        public override BaseItem RepresentItem
        {
            get
            {
                return GameInstance.Singleton.GoldDropRepresentItem;
            }
        }

        public static UniTask<GoldDropEntity> Drop(BaseGameEntity dropper, float multiplier, RewardGivenType givenType, int giverLevel, int sourceLevel, int amount, IEnumerable<string> looters)
        {
            return Drop(dropper, multiplier, givenType, giverLevel, sourceLevel, amount, looters, GameInstance.Singleton.itemAppearDuration);
        }

        public static async UniTask<GoldDropEntity> Drop(BaseGameEntity dropper, float multiplier, RewardGivenType givenType, int giverLevel, int sourceLevel, int amount, IEnumerable<string> looters, float appearDuration)
        {
            GoldDropEntity entity = null;
            GoldDropEntity loadedPrefab = await GameInstance.Singleton.GetLoadedGoldDropEntityPrefab();
            if (loadedPrefab != null)
            {
                entity = Drop(loadedPrefab, dropper, multiplier, givenType, giverLevel, sourceLevel, amount, looters, appearDuration);
            }
            return entity;
        }

        protected override bool ProceedPickingUpAtServer_Implementation(BaseCharacterEntity characterEntity, out UITextKeys message)
        {
            BaseCharacterEntity rewardingCharacter = characterEntity;
            if (characterEntity is BaseMonsterCharacterEntity monsterCharacterEntity && monsterCharacterEntity.Summoner is BasePlayerCharacterEntity summonerCharacterEntity)
                rewardingCharacter = summonerCharacterEntity;
            CurrentGameplayRule.RewardGold(rewardingCharacter, Amount, Multiplier, GivenType, GiverLevel, SourceLevel, out int rewardedGold);
            rewardingCharacter.OnRewardGold(GivenType, rewardedGold);
            message = UITextKeys.NONE;
            return true;
        }
    }
}
