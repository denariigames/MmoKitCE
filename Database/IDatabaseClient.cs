using Cysharp.Threading.Tasks;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabaseClient
    {
        UniTask<DatabaseApiResult<ValidateUserLoginResp>> ValidateUserLoginAsync(ValidateUserLoginReq request);

        UniTask<DatabaseApiResult<ValidateAccessTokenResp>> ValidateAccessTokenAsync(ValidateAccessTokenReq request);

        UniTask<DatabaseApiResult<GetUserLevelResp>> GetUserLevelAsync(GetUserLevelReq request);

        UniTask<DatabaseApiResult<GoldResp>> GetGoldAsync(GetGoldReq request);

        UniTask<DatabaseApiResult<GoldResp>> ChangeGoldAsync(ChangeGoldReq request);

        UniTask<DatabaseApiResult<CashResp>> GetCashAsync(GetCashReq request);

        UniTask<DatabaseApiResult<CashResp>> ChangeCashAsync(ChangeCashReq request);

        UniTask<DatabaseApiResult> UpdateAccessTokenAsync(UpdateAccessTokenReq request);

        UniTask<DatabaseApiResult> CreateUserLoginAsync(CreateUserLoginReq request);

        UniTask<DatabaseApiResult<FindUsernameResp>> FindUsernameAsync(FindUsernameReq request);

        UniTask<DatabaseApiResult<CharacterResp>> CreateCharacterAsync(CreateCharacterReq request);

        UniTask<DatabaseApiResult<CharacterResp>> GetCharacterAsync(GetCharacterReq request);

        UniTask<DatabaseApiResult<CharactersResp>> GetCharactersAsync(GetCharactersReq request);

        UniTask<DatabaseApiResult<CharacterResp>> UpdateCharacterAsync(UpdateCharacterReq request);

        UniTask<DatabaseApiResult> DeleteCharacterAsync(DeleteCharacterReq request);

        UniTask<DatabaseApiResult<FindCharacterNameResp>> FindCharacterNameAsync(FindCharacterNameReq request);

        UniTask<DatabaseApiResult<SocialCharacterResp>> GetSocialCharacterAsync(GetSocialCharacterReq request);

        UniTask<DatabaseApiResult<SocialCharactersResp>> FindCharactersAsync(FindCharacterNameReq request);

        UniTask<DatabaseApiResult> CreateFriendAsync(CreateFriendReq request);

        UniTask<DatabaseApiResult> DeleteFriendAsync(DeleteFriendReq request);

        UniTask<DatabaseApiResult<SocialCharactersResp>> GetFriendsAsync(GetFriendsReq request);

        UniTask<DatabaseApiResult<BuildingResp>> CreateBuildingAsync(CreateBuildingReq request);

        UniTask<DatabaseApiResult<BuildingResp>> UpdateBuildingAsync(UpdateBuildingReq request);

        UniTask<DatabaseApiResult> DeleteBuildingAsync(DeleteBuildingReq request);

        UniTask<DatabaseApiResult<BuildingsResp>> GetBuildingsAsync(GetBuildingsReq request);

        UniTask<DatabaseApiResult<PartyResp>> CreatePartyAsync(CreatePartyReq request);

        UniTask<DatabaseApiResult<PartyResp>> UpdatePartyAsync(UpdatePartyReq request);

        UniTask<DatabaseApiResult<PartyResp>> UpdatePartyLeaderAsync(UpdatePartyLeaderReq request);

        UniTask<DatabaseApiResult> DeletePartyAsync(DeletePartyReq request);

        UniTask<DatabaseApiResult<PartyResp>> UpdateCharacterPartyAsync(UpdateCharacterPartyReq request);

        UniTask<DatabaseApiResult> ClearCharacterPartyAsync(ClearCharacterPartyReq request);

        UniTask<DatabaseApiResult<PartyResp>> GetPartyAsync(GetPartyReq request);

        UniTask<DatabaseApiResult<GuildResp>> CreateGuildAsync(CreateGuildReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildLeaderAsync(UpdateGuildLeaderReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildMessageAsync(UpdateGuildMessageReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildMessage2Async(UpdateGuildMessageReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildOptionsAsync(UpdateGuildOptionsReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildAutoAcceptRequestsAsync(UpdateGuildAutoAcceptRequestsReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildRoleAsync(UpdateGuildRoleReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateGuildMemberRoleAsync(UpdateGuildMemberRoleReq request);

        UniTask<DatabaseApiResult> DeleteGuildAsync(DeleteGuildReq request);

        UniTask<DatabaseApiResult<GuildResp>> UpdateCharacterGuildAsync(UpdateCharacterGuildReq request);

        UniTask<DatabaseApiResult> ClearCharacterGuildAsync(ClearCharacterGuildReq request);

        UniTask<DatabaseApiResult<FindGuildNameResp>> FindGuildNameAsync(FindGuildNameReq request);

        UniTask<DatabaseApiResult<GuildsResp>> FindGuildsAsync(FindGuildNameReq request);

        UniTask<DatabaseApiResult<GuildResp>> GetGuildAsync(GetGuildReq request);

        UniTask<DatabaseApiResult<GuildResp>> IncreaseGuildExpAsync(IncreaseGuildExpReq request);

        UniTask<DatabaseApiResult<GuildResp>> AddGuildSkillAsync(AddGuildSkillReq request);

        UniTask<DatabaseApiResult<GuildGoldResp>> GetGuildGoldAsync(GetGuildGoldReq request);

        UniTask<DatabaseApiResult<GuildGoldResp>> ChangeGuildGoldAsync(ChangeGuildGoldReq request);

        UniTask<DatabaseApiResult> CreateGuildRequestAsync(CreateGuildRequestReq request);

        UniTask<DatabaseApiResult> DeleteGuildRequestAsync(DeleteGuildRequestReq request);

        UniTask<DatabaseApiResult<SocialCharactersResp>> GetGuildRequestsAsync(GetGuildRequestsReq request);

        UniTask<DatabaseApiResult<GetGuildRequestNotificationResp>> GetGuildRequestNotificationAsync(GetGuildRequestNotificationReq request);

        UniTask<DatabaseApiResult<GetStorageItemsResp>> GetStorageItemsAsync(GetStorageItemsReq request);

        UniTask<DatabaseApiResult> UpdateStorageItemsAsync(UpdateStorageItemsReq request);

        UniTask<DatabaseApiResult> UpdateStorageAndCharacterItemsAsync(UpdateStorageAndCharacterItemsReq request);

        UniTask<DatabaseApiResult> DeleteAllReservedStorageAsync();

        UniTask<DatabaseApiResult<MailListResp>> MailListAsync(MailListReq request);

        UniTask<DatabaseApiResult<UpdateReadMailStateResp>> UpdateReadMailStateAsync(UpdateReadMailStateReq request);

        UniTask<DatabaseApiResult<UpdateClaimMailItemsStateResp>> UpdateClaimMailItemsStateAsync(UpdateClaimMailItemsStateReq request);

        UniTask<DatabaseApiResult<UpdateDeleteMailStateResp>> UpdateDeleteMailStateAsync(UpdateDeleteMailStateReq request);

        UniTask<DatabaseApiResult<SendMailResp>> SendMailAsync(SendMailReq request);

        UniTask<DatabaseApiResult<GetMailResp>> GetMailAsync(GetMailReq request);

        UniTask<DatabaseApiResult<GetIdByCharacterNameResp>> GetIdByCharacterNameAsync(GetIdByCharacterNameReq request);

        UniTask<DatabaseApiResult<GetUserIdByCharacterNameResp>> GetUserIdByCharacterNameAsync(GetUserIdByCharacterNameReq request);

        UniTask<DatabaseApiResult<GetMailNotificationResp>> GetMailNotificationAsync(GetMailNotificationReq request);

        UniTask<DatabaseApiResult<GetUserUnbanTimeResp>> GetUserUnbanTimeAsync(GetUserUnbanTimeReq request);

        UniTask<DatabaseApiResult> SetUserUnbanTimeByCharacterNameAsync(SetUserUnbanTimeByCharacterNameReq request);

        UniTask<DatabaseApiResult> SetCharacterUnmuteTimeByNameAsync(SetCharacterUnmuteTimeByNameReq request);

        UniTask<DatabaseApiResult<GetSummonBuffsResp>> GetSummonBuffsAsync(GetSummonBuffsReq request);

        UniTask<DatabaseApiResult<ValidateEmailVerificationResp>> ValidateEmailVerificationAsync(ValidateEmailVerificationReq request);

        UniTask<DatabaseApiResult<FindEmailResp>> FindEmailAsync(FindEmailReq request);

        UniTask<DatabaseApiResult<GetFriendRequestNotificationResp>> GetFriendRequestNotificationAsync(GetFriendRequestNotificationReq request);

        UniTask<DatabaseApiResult<UserUnlockContentsResp>> GetUserUnlockContentsAsync(GetUserUnlockContentsReq request);

        UniTask<DatabaseApiResult<UserUnlockContentsResp>> GetAllUserUnlockContentsAsync(GetAllUserUnlockContentsReq request);

        UniTask<DatabaseApiResult<UserUnlockContentResp>> ChangeUserUnlockContentProgressionAsync(ChangeUserUnlockContentProgressionReq request);

        UniTask<DatabaseApiResult> UpdateUserUnlockContentAsync(UpdateUserUnlockContentReq request);

        UniTask<DatabaseApiResult<UserUnlockContentResp>> GetUserUnlockContentAsync(GetUserUnlockContentReq request);

        UniTask<DatabaseApiResult> UpdateUserCount(UpdateUserCountReq request);
        
        UniTask<DatabaseApiResult> UpdateGuildMemberCountAsync(UpdateGuildMemberCountReq request);

        UniTask<DatabaseApiResult> RemoveGuildCacheAsync(GetGuildReq request);

        UniTask<DatabaseApiResult> RemovePartyCacheAsync(GetPartyReq request);
    }
}
