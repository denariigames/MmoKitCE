using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterCurrency
    {
        public Currency GetCurrency()
        {
            if (GameInstance.Currencies.TryGetValue(dataId, out Currency result))
                return result;
            return null;
        }

        public static CharacterCurrency Create(Currency currency, int amount = 0)
        {
            return Create(currency.DataId, amount);
        }
    }

    [System.Serializable]
    public class SyncListCharacterCurrency : LiteNetLibSyncList<CharacterCurrency>
    {
    }
}
