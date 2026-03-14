namespace MultiplayerARPG
{
    public partial class CurrencyDropEntity
    {
        public override void Clean()
        {
            base.Clean();
            for (int i = 0; i < currencyAppearanceSettings.Count; ++i)
            {
                currencyAppearanceSettings[i].Clean();
            }
            currencyAppearanceSettings?.Clear();
            _allCurrencyActivatingObjects?.Clear();
            _currencyAppearanceSettings?.Clear();
        }
    }
}