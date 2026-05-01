// ce scability: #53

using System;
using System.Collections.Concurrent;
using System.Linq;
using Insthync.DevExtension;
using LiteNetLibManager;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using UnityEngine;
#endif

namespace MultiplayerARPG.MMO
{
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
    [DefaultExecutionOrder(DefaultExecutionOrders.DATABASE_NETWORK_MANAGER)]
#endif
    public partial class DatabaseNetworkManager : LiteNetLibManager.LiteNetLibManager
    {
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private BaseDatabase database = null;
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private BaseDatabase[] databaseOptions = new BaseDatabase[0];

        public BaseDatabase Database
        {
            get
            {
                return database == null ? databaseOptions.FirstOrDefault() : database;
            }
            set
            {
                database = value;
            }
        }

        public IDatabaseCache DatabaseCache
        {
            get; set;
        }

#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private bool useDeferredCharacterSaveScheduler = true;
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private int characterSaveLaneCount = 32;
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private int characterSaveTickMilliseconds = 500;
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private int characterSaveMaxPerTick = 8;
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private float characterSaveMinIntervalSeconds = 20f;
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        [SerializeField]
#endif
        private float characterSaveRetryDelaySeconds = 5f;

        private DatabaseCharacterSaveScheduler _characterSaveScheduler;
        private readonly ConcurrentDictionary<string, List<PlayerCharacterData>> _charactersByUserIdCache = new ConcurrentDictionary<string, List<PlayerCharacterData>>();


        public bool UseDeferredCharacterSaveScheduler => useDeferredCharacterSaveScheduler;

        public bool ProceedingBeforeQuit { get; private set; } = false;
        public bool ReadyToQuit { get; private set; } = false;

        public void SetDatabaseByOptionIndex(int index)
        {
            if (databaseOptions != null &&
                databaseOptions.Length > 0 &&
                index >= 0 &&
                index < databaseOptions.Length)
                database = databaseOptions[index];
        }

#if NET || NETCOREAPP
        public DatabaseNetworkManager() : base()
        {
            Initialize();
        }
#endif

#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
        protected override void Start()
        {
            Initialize();
            base.Start();
        }
#endif

        protected virtual void Initialize()
        {
            useWebSocket = false;
            maxConnections = int.MaxValue;
        }

        public async void ProceedBeforeQuit()
        {
            if (ProceedingBeforeQuit)
                return;
            ProceedingBeforeQuit = true;
            // Delay 30 secs before quit
            int seconds = 30;
            do
            {
                Logging.Log($"[DatabaseNetworkManager] {seconds} seconds before quit.");
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
                await UniTask.Delay(1000);
#elif NET || NETCOREAPP
                await Task.Delay(1000);
#endif
                seconds--;
            } while (seconds > 0);
            await FlushCharacterSavesAsync();
            await UniTask.Yield();
            ReadyToQuit = true;
            // Request to quit again
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
            Application.Quit();
#endif
        }

        public override async void OnStartServer()
        {
            base.OnStartServer();
#if (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
            Database.Initialize();
#endif
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
            await Database.DoMigration();

            if (useDeferredCharacterSaveScheduler)
            {
                _characterSaveScheduler = new DatabaseCharacterSaveScheduler(
                    this,
                    characterSaveLaneCount,
                    characterSaveTickMilliseconds,
                    characterSaveMaxPerTick,
                    characterSaveMinIntervalSeconds,
                    characterSaveRetryDelaySeconds);
                _characterSaveScheduler.Start();

                Logging.Log(nameof(DatabaseNetworkManager),
                    $"Character save scheduler started. lanes={characterSaveLaneCount}, tickMs={characterSaveTickMilliseconds}, maxPerTick={characterSaveMaxPerTick}, minInterval={characterSaveMinIntervalSeconds}s");
            }
#endif
        }

        public void EnqueueCharacterSave(UpdateCharacterReq request, bool forceImmediate = false)
        {
            if (_characterSaveScheduler == null)
                return;

            _characterSaveScheduler.Enqueue(request, forceImmediate);
        }

        public async UniTask<bool> InternalPersistCharacterUpdate(UpdateCharacterReq request)
        {
            try
            {
                await Database.UpdateCharacter(
                    request.State,
                    request.CharacterData,
                    request.SummonBuffs,
                    request.DeleteStorageReservation);

                await UniTask.WhenAll(
                    DatabaseCache.SetPlayerCharacter(request.CharacterData),
                    DatabaseCache.SetSummonBuffs(request.CharacterData.Id, request.SummonBuffs));

                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError(nameof(DatabaseNetworkManager),
                    $"Deferred character save failed for characterId={request.CharacterData.Id}. {ex}");
                return false;
            }
        }

        public async UniTask FlushCharacterSavesAsync()
        {
            if (_characterSaveScheduler != null)
                await _characterSaveScheduler.FlushAllAsync();
        }

        public bool TryGetCharactersByUserIdFromCache(string userId, out List<PlayerCharacterData> characters)
        {
            if (_charactersByUserIdCache.TryGetValue(userId, out characters))
            {
                if (characters == null)
                {
                    characters = new List<PlayerCharacterData>();
                    return true;
                }
                characters = new List<PlayerCharacterData>(characters);
                return true;
            }
            characters = null;
            return false;
        }

        public void SetCharactersByUserIdCache(string userId, List<PlayerCharacterData> characters)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            _charactersByUserIdCache[userId] = characters == null
                ? new List<PlayerCharacterData>()
                : new List<PlayerCharacterData>(characters);
        }

        public void InvalidateCharactersByUserIdCache(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            _charactersByUserIdCache.TryRemove(userId, out _);
        }

        public void UpsertCharacterInUserIdCache(PlayerCharacterData characterData)
        {
            if (characterData == null || string.IsNullOrEmpty(characterData.UserId) || string.IsNullOrEmpty(characterData.Id))
                return;

            _charactersByUserIdCache.AddOrUpdate(
                characterData.UserId,
                _ => new List<PlayerCharacterData>() { characterData },
                (_, existing) =>
                {
                    List<PlayerCharacterData> list = existing == null
                        ? new List<PlayerCharacterData>()
                        : new List<PlayerCharacterData>(existing);

                    int index = list.FindIndex(c => c != null && c.Id == characterData.Id);
                    if (index >= 0)
                        list[index] = characterData;
                    else
                        list.Add(characterData);

                    return list;
                });
        }

        public void RemoveCharacterFromUserIdCache(string userId, string characterId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(characterId))
                return;

            _charactersByUserIdCache.AddOrUpdate(
                userId,
                _ => new List<PlayerCharacterData>(),
                (_, existing) =>
                {
                    List<PlayerCharacterData> list = existing == null
                        ? new List<PlayerCharacterData>()
                        : new List<PlayerCharacterData>(existing);

                    list.RemoveAll(c => c != null && c.Id == characterId);
                    return list;
                });
        }

        protected override void RegisterMessages()
        {
            base.RegisterMessages();
            EnableRequestResponse(MMOMessageTypes.Request, MMOMessageTypes.Response);
            RegisterRequestToServer<DbRequestMessage<ValidateUserLoginReq>, ValidateUserLoginResp>(DatabaseRequestTypes.ValidateUserLogin, ValidateUserLogin);
            RegisterRequestToServer<DbRequestMessage<ValidateAccessTokenReq>, ValidateAccessTokenResp>(DatabaseRequestTypes.ValidateAccessToken, ValidateAccessToken);
            RegisterRequestToServer<DbRequestMessage<GetUserLevelReq>, GetUserLevelResp>(DatabaseRequestTypes.GetUserLevel, GetUserLevel);
            RegisterRequestToServer<DbRequestMessage<GetGoldReq>, GoldResp>(DatabaseRequestTypes.GetGold, GetGold);
            RegisterRequestToServer<DbRequestMessage<ChangeGoldReq>, GoldResp>(DatabaseRequestTypes.ChangeGold, ChangeGold);
            RegisterRequestToServer<DbRequestMessage<GetCashReq>, CashResp>(DatabaseRequestTypes.GetCash, GetCash);
            RegisterRequestToServer<DbRequestMessage<ChangeCashReq>, CashResp>(DatabaseRequestTypes.ChangeCash, ChangeCash);
            RegisterRequestToServer<DbRequestMessage<UpdateAccessTokenReq>, EmptyMessage>(DatabaseRequestTypes.UpdateAccessToken, UpdateAccessToken);
            RegisterRequestToServer<DbRequestMessage<CreateUserLoginReq>, EmptyMessage>(DatabaseRequestTypes.CreateUserLogin, CreateUserLogin);
            RegisterRequestToServer<DbRequestMessage<FindUsernameReq>, FindUsernameResp>(DatabaseRequestTypes.FindUsername, FindUsername);
            RegisterRequestToServer<DbRequestMessage<CreateCharacterReq>, CharacterResp>(DatabaseRequestTypes.CreateCharacter, CreateCharacter);
            RegisterRequestToServer<DbRequestMessage<GetCharacterReq>, CharacterResp>(DatabaseRequestTypes.GetCharacter, GetCharacter);
            RegisterRequestToServer<DbRequestMessage<GetCharactersReq>, CharactersResp>(DatabaseRequestTypes.GetCharacters, GetCharacters);
            RegisterRequestToServer<DbRequestMessage<UpdateCharacterReq>, CharacterResp>(DatabaseRequestTypes.UpdateCharacter, UpdateCharacter);
            RegisterRequestToServer<DbRequestMessage<DeleteCharacterReq>, EmptyMessage>(DatabaseRequestTypes.DeleteCharacter, DeleteCharacter);
            RegisterRequestToServer<DbRequestMessage<FindCharacterNameReq>, FindCharacterNameResp>(DatabaseRequestTypes.FindCharacterName, FindCharacterName);
            RegisterRequestToServer<DbRequestMessage<FindCharacterNameReq>, SocialCharactersResp>(DatabaseRequestTypes.FindCharacters, FindCharacters);
            RegisterRequestToServer<DbRequestMessage<CreateFriendReq>, EmptyMessage>(DatabaseRequestTypes.CreateFriend, CreateFriend);
            RegisterRequestToServer<DbRequestMessage<DeleteFriendReq>, EmptyMessage>(DatabaseRequestTypes.DeleteFriend, DeleteFriend);
            RegisterRequestToServer<DbRequestMessage<GetFriendsReq>, SocialCharactersResp>(DatabaseRequestTypes.GetFriends, GetFriends);
            RegisterRequestToServer<DbRequestMessage<GetFriendRequestNotificationReq>, GetFriendRequestNotificationResp>(DatabaseRequestTypes.GetFriendRequestNotification, GetFriendRequestNotification);
            RegisterRequestToServer<DbRequestMessage<CreateBuildingReq>, BuildingResp>(DatabaseRequestTypes.CreateBuilding, CreateBuilding);
            RegisterRequestToServer<DbRequestMessage<UpdateBuildingReq>, BuildingResp>(DatabaseRequestTypes.UpdateBuilding, UpdateBuilding);
            RegisterRequestToServer<DbRequestMessage<DeleteBuildingReq>, EmptyMessage>(DatabaseRequestTypes.DeleteBuilding, DeleteBuilding);
            RegisterRequestToServer<DbRequestMessage<GetBuildingsReq>, BuildingsResp>(DatabaseRequestTypes.GetBuildings, GetBuildings);
            RegisterRequestToServer<DbRequestMessage<CreatePartyReq>, PartyResp>(DatabaseRequestTypes.CreateParty, CreateParty);
            RegisterRequestToServer<DbRequestMessage<UpdatePartyReq>, PartyResp>(DatabaseRequestTypes.UpdateParty, UpdateParty);
            RegisterRequestToServer<DbRequestMessage<UpdatePartyLeaderReq>, PartyResp>(DatabaseRequestTypes.UpdatePartyLeader, UpdatePartyLeader);
            RegisterRequestToServer<DbRequestMessage<DeletePartyReq>, EmptyMessage>(DatabaseRequestTypes.DeleteParty, DeleteParty);
            RegisterRequestToServer<DbRequestMessage<UpdateCharacterPartyReq>, PartyResp>(DatabaseRequestTypes.UpdateCharacterParty, UpdateCharacterParty);
            RegisterRequestToServer<DbRequestMessage<ClearCharacterPartyReq>, EmptyMessage>(DatabaseRequestTypes.ClearCharacterParty, ClearCharacterParty);
            RegisterRequestToServer<DbRequestMessage<GetPartyReq>, PartyResp>(DatabaseRequestTypes.GetParty, GetParty);
            RegisterRequestToServer<DbRequestMessage<CreateGuildReq>, GuildResp>(DatabaseRequestTypes.CreateGuild, CreateGuild);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildLeaderReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildLeader, UpdateGuildLeader);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildMessageReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildMessage, UpdateGuildMessage);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildMessageReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildMessage2, UpdateGuildMessage2);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildScoreReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildScore, UpdateGuildScore);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildOptionsReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildOptions, UpdateGuildOptions);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildAutoAcceptRequestsReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildAutoAcceptRequests, UpdateGuildAutoAcceptRequests);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildRankReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildRank, UpdateGuildRank);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildRoleReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildRole, UpdateGuildRole);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildMemberRoleReq>, GuildResp>(DatabaseRequestTypes.UpdateGuildMemberRole, UpdateGuildMemberRole);
            RegisterRequestToServer<DbRequestMessage<DeleteGuildReq>, EmptyMessage>(DatabaseRequestTypes.DeleteGuild, DeleteGuild);
            RegisterRequestToServer<DbRequestMessage<UpdateCharacterGuildReq>, GuildResp>(DatabaseRequestTypes.UpdateCharacterGuild, UpdateCharacterGuild);
            RegisterRequestToServer<DbRequestMessage<ClearCharacterGuildReq>, EmptyMessage>(DatabaseRequestTypes.ClearCharacterGuild, ClearCharacterGuild);
            RegisterRequestToServer<DbRequestMessage<FindGuildNameReq>, FindGuildNameResp>(DatabaseRequestTypes.FindGuildName, FindGuildName);
            RegisterRequestToServer<DbRequestMessage<GetGuildReq>, GuildResp>(DatabaseRequestTypes.GetGuild, GetGuild);
            RegisterRequestToServer<DbRequestMessage<IncreaseGuildExpReq>, GuildResp>(DatabaseRequestTypes.IncreaseGuildExp, IncreaseGuildExp);
            RegisterRequestToServer<DbRequestMessage<AddGuildSkillReq>, GuildResp>(DatabaseRequestTypes.AddGuildSkill, AddGuildSkill);
            RegisterRequestToServer<DbRequestMessage<GetGuildGoldReq>, GuildGoldResp>(DatabaseRequestTypes.GetGuildGold, GetGuildGold);
            RegisterRequestToServer<DbRequestMessage<ChangeGuildGoldReq>, GuildGoldResp>(DatabaseRequestTypes.ChangeGuildGold, ChangeGuildGold);
            RegisterRequestToServer<DbRequestMessage<GetStorageItemsReq>, GetStorageItemsResp>(DatabaseRequestTypes.GetStorageItems, GetStorageItems);
            RegisterRequestToServer<DbRequestMessage<UpdateStorageItemsReq>, EmptyMessage>(DatabaseRequestTypes.UpdateStorageItems, UpdateStorageItems);
            RegisterRequestToServer<DbRequestMessage<UpdateStorageAndCharacterItemsReq>, EmptyMessage>(DatabaseRequestTypes.UpdateStorageAndCharacterItems, UpdateStorageAndCharacterItems);
            RegisterRequestToServer<DbRequestMessage<EmptyMessage>, EmptyMessage>(DatabaseRequestTypes.DeleteAllReservedStorage, DeleteAllReservedStorage);
            RegisterRequestToServer<DbRequestMessage<MailListReq>, MailListResp>(DatabaseRequestTypes.MailList, MailList);
            RegisterRequestToServer<DbRequestMessage<UpdateReadMailStateReq>, UpdateReadMailStateResp>(DatabaseRequestTypes.UpdateReadMailState, UpdateReadMailState);
            RegisterRequestToServer<DbRequestMessage<UpdateClaimMailItemsStateReq>, UpdateClaimMailItemsStateResp>(DatabaseRequestTypes.UpdateClaimMailItemsState, UpdateClaimMailItemsState);
            RegisterRequestToServer<DbRequestMessage<UpdateDeleteMailStateReq>, UpdateDeleteMailStateResp>(DatabaseRequestTypes.UpdateDeleteMailState, UpdateDeleteMailState);
            RegisterRequestToServer<DbRequestMessage<SendMailReq>, SendMailResp>(DatabaseRequestTypes.SendMail, SendMail);
            RegisterRequestToServer<DbRequestMessage<GetMailReq>, GetMailResp>(DatabaseRequestTypes.GetMail, GetMail);
            RegisterRequestToServer<DbRequestMessage<GetMailNotificationReq>, GetMailNotificationResp>(DatabaseRequestTypes.GetMailNotification, GetMailNotification);
            RegisterRequestToServer<DbRequestMessage<GetIdByCharacterNameReq>, GetIdByCharacterNameResp>(DatabaseRequestTypes.GetIdByCharacterName, GetIdByCharacterName);
            RegisterRequestToServer<DbRequestMessage<GetUserIdByCharacterNameReq>, GetUserIdByCharacterNameResp>(DatabaseRequestTypes.GetUserIdByCharacterName, GetUserIdByCharacterName);
            RegisterRequestToServer<DbRequestMessage<GetUserUnbanTimeReq>, GetUserUnbanTimeResp>(DatabaseRequestTypes.GetUserUnbanTime, GetUserUnbanTime);
            RegisterRequestToServer<DbRequestMessage<SetUserUnbanTimeByCharacterNameReq>, EmptyMessage>(DatabaseRequestTypes.SetUserUnbanTimeByCharacterName, SetUserUnbanTimeByCharacterName);
            RegisterRequestToServer<DbRequestMessage<SetCharacterUnmuteTimeByNameReq>, EmptyMessage>(DatabaseRequestTypes.SetCharacterUnmuteTimeByName, SetCharacterUnmuteTimeByName);
            RegisterRequestToServer<DbRequestMessage<GetSummonBuffsReq>, GetSummonBuffsResp>(DatabaseRequestTypes.GetSummonBuffs, GetSummonBuffs);
            RegisterRequestToServer<DbRequestMessage<FindEmailReq>, FindEmailResp>(DatabaseRequestTypes.FindEmail, FindEmail);
            RegisterRequestToServer<DbRequestMessage<ValidateEmailVerificationReq>, ValidateEmailVerificationResp>(DatabaseRequestTypes.ValidateEmailVerification, ValidateEmailVerification);
            RegisterRequestToServer<DbRequestMessage<UpdateUserCountReq>, EmptyMessage>(DatabaseRequestTypes.UpdateUserCount, UpdateUserCount);
            RegisterRequestToServer<DbRequestMessage<GetSocialCharacterReq>, SocialCharacterResp>(DatabaseRequestTypes.GetSocialCharacter, GetSocialCharacter);
            RegisterRequestToServer<DbRequestMessage<FindGuildNameReq>, GuildsResp>(DatabaseRequestTypes.FindGuilds, FindGuilds);
            RegisterRequestToServer<DbRequestMessage<CreateGuildRequestReq>, EmptyMessage>(DatabaseRequestTypes.CreateGuildRequest, CreateGuildRequest);
            RegisterRequestToServer<DbRequestMessage<DeleteGuildRequestReq>, EmptyMessage>(DatabaseRequestTypes.DeleteGuildRequest, DeleteGuildRequest);
            RegisterRequestToServer<DbRequestMessage<GetGuildRequestsReq>, SocialCharactersResp>(DatabaseRequestTypes.GetGuildRequests, GetGuildRequests);
            RegisterRequestToServer<DbRequestMessage<GetGuildRequestNotificationReq>, GetGuildRequestNotificationResp>(DatabaseRequestTypes.GetGuildRequestNotification, GetGuildRequestNotification);
            RegisterRequestToServer<DbRequestMessage<UpdateGuildMemberCountReq>, EmptyMessage>(DatabaseRequestTypes.UpdateGuildMemberCount, UpdateGuildMemberCount);
            this.InvokeInstanceDevExtMethods("RegisterMessages");
        }
    }
}