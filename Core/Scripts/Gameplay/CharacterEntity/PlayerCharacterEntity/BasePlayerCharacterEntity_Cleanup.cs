namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        public override void Clean()
        {
            base.Clean();
            characterDatabases?.Nullify();
            characterDatabases = null;
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            controllerPrefab = null;
#endif
#if !DISABLE_ADDRESSABLES
            addressableControllerPrefab = null;
#endif
            ItemLockAndExpireComponent = null;
            NpcActionComponent = null;
            BuildingComponent = null;
            CraftingComponent = null;
            DealingComponent = null;
            DuelingComponent = null;
            VendingComponent = null;
            PkComponent = null;
            // Events
            onDataIdChange = null;
            onFactionIdChange = null;
            onStatPointChange = null;
            onSkillPointChange = null;
            onGoldChange = null;
            onUserGoldChange = null;
            onUserCashChange = null;
            onPartyIdChange = null;
            onGuildIdChange = null;
            onIconDataIdChange = null;
            onFrameDataIdChange = null;
            onTitleDataIdChange = null;
#if !DISABLE_CLASSIC_PK
            onIsPkOnChange = null;
            onPkPointChange = null;
            onConsecutivePkKillsChange = null;
#endif
            onIsWarpingChange = null;
            onHotkeysOperation = null;
            onQuestsOperation = null;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            onCurrenciesOperation = null;
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            onServerBoolsOperation = null;
            onServerIntsOperation = null;
            onServerFloatsOperation = null;
            onPrivateBoolsOperation = null;
            onPrivateIntsOperation = null;
            onPrivateFloatsOperation = null;
            onPublicBoolsOperation = null;
            onPublicIntsOperation = null;
            onPublicFloatsOperation = null;
#endif
        }
    }
}
