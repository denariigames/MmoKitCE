using System.Collections.Generic;

namespace MultiplayerARPG
{
    public partial class PartyData
    {
        public void GetSortedMembers(out SocialCharacterData[] sortedMembers)
        {
            sortedMembers = new SocialCharacterData[members.Count];
            if (members.Count <= 0)
                return;
            int i = 0;
            sortedMembers[i++] = members[leaderId];
            List<SocialCharacterData> offlineMembers = new List<SocialCharacterData>();
            SocialCharacterData tempMember;
            foreach (string memberId in members.Keys)
            {
                if (memberId.Equals(leaderId))
                    continue;
                tempMember = members[memberId];
                if (!GameInstance.ClientOnlineCharacterHandlers.IsCharacterOnline(memberId))
                {
                    offlineMembers.Add(tempMember);
                    continue;
                }
                sortedMembers[i++] = tempMember;
            }
            foreach (SocialCharacterData offlineMember in offlineMembers)
            {
                sortedMembers[i++] = offlineMember;
            }
        }

        public int MaxMember()
        {
            return SystemSetting.MaxPartyMember;
        }

        public bool CanInvite(string characterId)
        {
            if (IsLeader(characterId))
                return true;
            else
                return SystemSetting.PartyMemberCanInvite;
        }

        public bool CanKick(string characterId)
        {
            if (IsLeader(characterId))
                return true;
            else
                return SystemSetting.PartyMemberCanKick;
        }
    }
}
