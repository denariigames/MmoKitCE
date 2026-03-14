using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class Reward : System.IDisposable
    {
        public int exp;
        public int gold;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        public List<CurrencyAmount> currencies;
#endif

        ~Reward()
        {
            Dispose();
        }

        public bool NoExp()
        {
            return exp <= 0;
        }

        public bool NoGold()
        {
            return gold <= 0;
        }

        public bool NoCurrencies()
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            return currencies == null || currencies.Count == 0;
#else
            return true;
#endif
        }

        public bool NoRewards()
        {
            return NoExp() && NoGold() && NoCurrencies();
        }

        public void Dispose()
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            currencies?.Clear();
            currencies = null;
#endif
        }
    }
}
