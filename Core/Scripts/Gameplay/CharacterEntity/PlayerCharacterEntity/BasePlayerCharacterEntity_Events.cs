using LiteNetLibManager;
using NotifiableCollection;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        // Note: You may use `Awake` dev extension to setup an events and `OnDestroy` to desetup an events
        // Sync variables
        public event CharacterEntityInt32ChangeDelegate onDataIdChange;
        public event CharacterEntityInt32ChangeDelegate onFactionIdChange;
        public event CharacterEntitySingleChangeDelegate onStatPointChange;
        public event CharacterEntitySingleChangeDelegate onSkillPointChange;
        public event CharacterEntityInt32ChangeDelegate onGoldChange;
        public event CharacterEntityInt32ChangeDelegate onUserGoldChange;
        public event CharacterEntityInt32ChangeDelegate onUserCashChange;
        public event CharacterEntityInt32ChangeDelegate onPartyIdChange;
        public event CharacterEntityInt32ChangeDelegate onGuildIdChange;
        public event CharacterEntityInt32ChangeDelegate onIconDataIdChange;
        public event CharacterEntityInt32ChangeDelegate onFrameDataIdChange;
        public event CharacterEntityInt32ChangeDelegate onBackgroundDataIdChange;
        public event CharacterEntityInt32ChangeDelegate onTitleDataIdChange;
#if !DISABLE_CLASSIC_PK
        public event CharacterEntityBooleanChangeDelegate onIsPkOnChange;
        public event CharacterEntityInt32ChangeDelegate onPkPointChange;
        public event CharacterEntityInt32ChangeDelegate onConsecutivePkKillsChange;
#endif
        public event CharacterEntityInt32ChangeDelegate onReputationChange;
        public event CharacterEntityBooleanChangeDelegate onIsWarpingChange;
        // Sync lists
        public event LiteNetLibSyncList<CharacterHotkey>.OnOperationDelegate onHotkeysOperation;
        public event LiteNetLibSyncList<CharacterQuest>.OnOperationDelegate onQuestsOperation;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        public event LiteNetLibSyncList<CharacterCurrency>.OnOperationDelegate onCurrenciesOperation;
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
        public event NotifiableList<CharacterDataBoolean>.OnChangedDelegate onServerBoolsOperation;
        public event NotifiableList<CharacterDataInt32>.OnChangedDelegate onServerIntsOperation;
        public event NotifiableList<CharacterDataFloat32>.OnChangedDelegate onServerFloatsOperation;
        public event LiteNetLibSyncList<CharacterDataBoolean>.OnOperationDelegate onPrivateBoolsOperation;
        public event LiteNetLibSyncList<CharacterDataInt32>.OnOperationDelegate onPrivateIntsOperation;
        public event LiteNetLibSyncList<CharacterDataFloat32>.OnOperationDelegate onPrivateFloatsOperation;
        public event LiteNetLibSyncList<CharacterDataBoolean>.OnOperationDelegate onPublicBoolsOperation;
        public event LiteNetLibSyncList<CharacterDataInt32>.OnOperationDelegate onPublicIntsOperation;
        public event LiteNetLibSyncList<CharacterDataFloat32>.OnOperationDelegate onPublicFloatsOperation;
#endif
        public event LiteNetLibSyncList<CharacterSkill>.OnOperationDelegate onGuildSkillsOperation;

        public override void OnRewardItem(RewardGivenType givenType, BaseItem item, int amount)
        {
            GameInstance.ServerGameMessageHandlers.NotifyRewardItem(ConnectionId, givenType, item.DataId, amount);
            GameInstance.ServerLogHandlers.LogRewardItem(this, givenType, item, amount);
        }

        public override void OnRewardItem(RewardGivenType givenType, CharacterItem item)
        {
            GameInstance.ServerGameMessageHandlers.NotifyRewardItem(ConnectionId, givenType, item.dataId, item.amount);
            GameInstance.ServerLogHandlers.LogRewardItem(this, givenType, item);
        }

        public override void OnRewardGold(RewardGivenType givenType, int gold)
        {
            GameInstance.ServerGameMessageHandlers.NotifyRewardGold(ConnectionId, givenType, gold);
            GameInstance.ServerLogHandlers.LogRewardGold(this, givenType, gold);
        }

        public override void OnRewardExp(RewardGivenType givenType, int exp, bool isLevelUp)
        {
            GameInstance.ServerGameMessageHandlers.NotifyRewardExp(ConnectionId, givenType, exp);
            GameInstance.ServerLogHandlers.LogRewardExp(this, givenType, exp, isLevelUp);
        }

        public override void OnRewardCurrency(RewardGivenType givenType, Currency currency, int amount)
        {
            GameInstance.ServerGameMessageHandlers.NotifyRewardCurrency(ConnectionId, givenType, currency.DataId, amount);
            GameInstance.ServerLogHandlers.LogRewardCurrency(this, givenType, currency, amount);
        }

        public override void OnRewardCurrency(RewardGivenType givenType, CharacterCurrency currency)
        {
            GameInstance.ServerGameMessageHandlers.NotifyRewardCurrency(ConnectionId, givenType, currency.dataId, currency.amount);
            GameInstance.ServerLogHandlers.LogRewardCurrency(this, givenType, currency);
        }
    }
}
