namespace MultiplayerARPG
{
    public struct UICharacterCurrencyData
    {
        public CharacterCurrency characterCurrency;
        public float targetAmount;
        public UICharacterCurrencyData(CharacterCurrency characterCurrency, float targetAmount)
        {
            this.characterCurrency = characterCurrency;
            this.targetAmount = targetAmount;
        }
        public UICharacterCurrencyData(CharacterCurrency characterCurrency) : this(characterCurrency, characterCurrency.amount)
        {
        }
        public UICharacterCurrencyData(Currency attribute, int targetAmount) : this(CharacterCurrency.Create(attribute, targetAmount), targetAmount)
        {
        }
    }
}
