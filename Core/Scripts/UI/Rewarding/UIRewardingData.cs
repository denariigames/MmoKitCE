using Insthync.UnityEditorUtils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct UIRewardingData
    {
        public int rewardExp;
        public int rewardGold;
        public int rewardCash;
        [ArrayElementTitle("item")]
        public List<RewardedItem> rewardItems;
    }
}
