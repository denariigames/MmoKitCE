using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.CURRENCY_FILE, menuName = GameDataMenuConsts.CURRENCY_MENU, order = GameDataMenuConsts.CURRENCY_ORDER)]
    public partial class Currency : BaseGameData
    {
    }

    [System.Serializable]
    public struct CurrencyAmount
    {
        public Currency currency;
        public int amount;
    }

    [System.Serializable]
    public struct CurrencyRandomAmount
    {
        public Currency currency;
        public int minAmount;
        public int maxAmount;

        public void GetMinMaxAmount(out int min, out int max)
        {
            min = minAmount;
            max = maxAmount;
            if (max <= 0)
                max = 1;
            if (max < min)
                max = min;
            if (min <= 0)
                min = max;
        }

        public int GetRandomedAmount()
        {
            GetMinMaxAmount(out int min, out int max);
            return Random.Range(min, max);
        }
    }

    [System.Serializable]
    public struct CurrencyItemPair
    {
        public Currency currency;
        public BaseItem item;
    }
}
