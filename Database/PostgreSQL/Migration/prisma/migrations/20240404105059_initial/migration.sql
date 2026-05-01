-- CreateTable
CREATE TABLE "buildings" (
    "id" VARCHAR(50) NOT NULL,
    "channel" VARCHAR(50) NOT NULL DEFAULT 'default',
    "parentId" VARCHAR(50) NOT NULL DEFAULT '',
    "entityId" INTEGER NOT NULL DEFAULT 0,
    "currentHp" INTEGER NOT NULL DEFAULT 0,
    "remainsLifeTime" REAL NOT NULL DEFAULT 0,
    "mapName" VARCHAR(50) NOT NULL,
    "positionX" REAL NOT NULL DEFAULT 0,
    "positionY" REAL NOT NULL DEFAULT 0,
    "positionZ" REAL NOT NULL DEFAULT 0,
    "rotationX" REAL NOT NULL DEFAULT 0,
    "rotationY" REAL NOT NULL DEFAULT 0,
    "rotationZ" REAL NOT NULL DEFAULT 0,
    "isLocked" BOOLEAN NOT NULL DEFAULT false,
    "lockPassword" VARCHAR(6) NOT NULL DEFAULT '',
    "creatorId" VARCHAR(50) NOT NULL DEFAULT '',
    "creatorName" VARCHAR(32) NOT NULL DEFAULT '',
    "extraData" TEXT NOT NULL,
    "isSceneObject" BOOLEAN NOT NULL DEFAULT false,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "buildings_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_pk" (
    "id" VARCHAR(255) NOT NULL,
    "isPkOn" BOOLEAN NOT NULL DEFAULT false,
    "lastPkOnTime" INTEGER NOT NULL DEFAULT 0,
    "pkPoint" INTEGER NOT NULL DEFAULT 0,
    "consecutivePkKills" INTEGER NOT NULL DEFAULT 0,
    "highestPkPoint" INTEGER NOT NULL DEFAULT 0,
    "highestConsecutivePkKills" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "character_pk_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_private_boolean" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" BOOLEAN NOT NULL DEFAULT false,

    CONSTRAINT "character_private_boolean_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_private_float32" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" REAL NOT NULL DEFAULT 0,

    CONSTRAINT "character_private_float32_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_private_int32" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "character_private_int32_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_public_boolean" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" BOOLEAN NOT NULL DEFAULT false,

    CONSTRAINT "character_public_boolean_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_public_float32" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" REAL NOT NULL DEFAULT 0,

    CONSTRAINT "character_public_float32_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_public_int32" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "character_public_int32_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_server_boolean" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" BOOLEAN NOT NULL DEFAULT false,

    CONSTRAINT "character_server_boolean_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_server_float32" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" REAL NOT NULL DEFAULT 0,

    CONSTRAINT "character_server_float32_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_server_int32" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hashedKey" INTEGER NOT NULL,
    "value" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "character_server_int32_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characterattribute" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "amount" INTEGER NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characterattribute_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characterbuff" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "type" SMALLINT NOT NULL DEFAULT 0,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "buffRemainsDuration" REAL NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characterbuff_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "charactercurrency" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "amount" INTEGER NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "charactercurrency_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characterhotkey" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "hotkeyId" VARCHAR(50) NOT NULL,
    "type" SMALLINT NOT NULL DEFAULT 0,
    "relateId" VARCHAR(50) NOT NULL,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characterhotkey_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characteritem" (
    "id" VARCHAR(50) NOT NULL,
    "idx" INTEGER NOT NULL,
    "inventoryType" SMALLINT NOT NULL DEFAULT 0,
    "characterId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "amount" INTEGER NOT NULL DEFAULT 0,
    "equipSlotIndex" SMALLINT NOT NULL DEFAULT 0,
    "durability" REAL NOT NULL DEFAULT 0,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "lockRemainsDuration" REAL NOT NULL DEFAULT 0,
    "expireTime" BIGINT NOT NULL DEFAULT 0,
    "randomSeed" INTEGER NOT NULL DEFAULT 0,
    "ammo" INTEGER NOT NULL DEFAULT 0,
    "sockets" TEXT NOT NULL,
    "version" SMALLINT NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characteritem_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characterquest" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "randomTasksIndex" SMALLINT NOT NULL DEFAULT 0,
    "isComplete" BOOLEAN NOT NULL DEFAULT false,
    "completeTime" BIGINT NOT NULL DEFAULT 0,
    "isTracking" BOOLEAN NOT NULL DEFAULT false,
    "killedMonsters" TEXT NOT NULL,
    "completedTasks" TEXT NOT NULL,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characterquest_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characters" (
    "id" VARCHAR(50) NOT NULL,
    "userId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "entityId" INTEGER NOT NULL DEFAULT 0,
    "factionId" INTEGER NOT NULL DEFAULT 0,
    "characterName" VARCHAR(32) NOT NULL DEFAULT '',
    "level" INTEGER NOT NULL DEFAULT 1,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "currentHp" INTEGER NOT NULL DEFAULT 0,
    "currentMp" INTEGER NOT NULL DEFAULT 0,
    "currentStamina" INTEGER NOT NULL DEFAULT 0,
    "currentFood" INTEGER NOT NULL DEFAULT 0,
    "currentWater" INTEGER NOT NULL DEFAULT 0,
    "equipWeaponSet" SMALLINT NOT NULL DEFAULT 0,
    "statPoint" REAL NOT NULL DEFAULT 0,
    "skillPoint" REAL NOT NULL DEFAULT 0,
    "gold" INTEGER NOT NULL DEFAULT 0,
    "partyId" INTEGER NOT NULL DEFAULT 0,
    "guildId" INTEGER NOT NULL DEFAULT 0,
    "guildRole" INTEGER NOT NULL DEFAULT 0,
    "sharedGuildExp" INTEGER NOT NULL DEFAULT 0,
    "currentMapName" VARCHAR(50) NOT NULL DEFAULT '',
    "currentPositionX" REAL NOT NULL DEFAULT 0,
    "currentPositionY" REAL NOT NULL DEFAULT 0,
    "currentPositionZ" REAL NOT NULL DEFAULT 0,
    "currentRotationX" REAL NOT NULL DEFAULT 0,
    "currentRotationY" REAL NOT NULL DEFAULT 0,
    "currentRotationZ" REAL NOT NULL DEFAULT 0,
    "respawnMapName" VARCHAR(50) NOT NULL DEFAULT '',
    "respawnPositionX" REAL NOT NULL DEFAULT 0,
    "respawnPositionY" REAL NOT NULL DEFAULT 0,
    "respawnPositionZ" REAL NOT NULL DEFAULT 0,
    "mountDataId" INTEGER NOT NULL DEFAULT 0,
    "iconDataId" INTEGER NOT NULL DEFAULT 0,
    "frameDataId" INTEGER NOT NULL DEFAULT 0,
    "titleDataId" INTEGER NOT NULL DEFAULT 0,
    "lastDeadTime" BIGINT NOT NULL DEFAULT 0,
    "unmuteTime" BIGINT NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characters_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characterskill" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characterskill_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "characterskillusage" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "type" SMALLINT NOT NULL DEFAULT 0,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "coolDownRemainsDuration" REAL NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "characterskillusage_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "charactersummon" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "type" SMALLINT NOT NULL DEFAULT 0,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "summonRemainsDuration" REAL NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 0,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "currentHp" INTEGER NOT NULL DEFAULT 0,
    "currentMp" INTEGER NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "charactersummon_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "friend" (
    "id" SERIAL NOT NULL,
    "characterId1" VARCHAR(50) NOT NULL,
    "characterId2" VARCHAR(50) NOT NULL,
    "state" BOOLEAN NOT NULL DEFAULT false,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "friend_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "guild" (
    "id" SERIAL NOT NULL,
    "guildName" VARCHAR(32) NOT NULL,
    "leaderId" VARCHAR(50) NOT NULL,
    "level" INTEGER NOT NULL DEFAULT 1,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "skillPoint" INTEGER NOT NULL DEFAULT 0,
    "guildMessage" VARCHAR(160) NOT NULL DEFAULT '',
    "guildMessage2" VARCHAR(160) NOT NULL DEFAULT '',
    "gold" INTEGER NOT NULL DEFAULT 0,
    "score" INTEGER NOT NULL DEFAULT 0,
    "options" TEXT NOT NULL,
    "autoAcceptRequests" BOOLEAN NOT NULL DEFAULT false,
    "rank" INTEGER NOT NULL DEFAULT 0,
    "currentMembers" INTEGER NOT NULL DEFAULT 0,
    "maxMembers" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "guild_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "guildrequest" (
    "id" SERIAL NOT NULL,
    "guildId" INTEGER NOT NULL,
    "requesterId" VARCHAR(50) NOT NULL,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "guildrequest_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "guildrole" (
    "guildId" INTEGER NOT NULL,
    "guildRole" INTEGER NOT NULL,
    "name" VARCHAR(50) NOT NULL,
    "canInvite" BOOLEAN NOT NULL DEFAULT false,
    "canKick" BOOLEAN NOT NULL DEFAULT false,
    "canUseStorage" BOOLEAN NOT NULL DEFAULT false,
    "shareExpPercentage" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "guildrole_pkey" PRIMARY KEY ("guildId","guildRole")
);

-- CreateTable
CREATE TABLE "guildskill" (
    "guildId" INTEGER NOT NULL,
    "dataId" INTEGER NOT NULL,
    "level" INTEGER NOT NULL,

    CONSTRAINT "guildskill_pkey" PRIMARY KEY ("guildId","dataId")
);

-- CreateTable
CREATE TABLE "mail" (
    "id" BIGSERIAL NOT NULL,
    "eventId" VARCHAR(50) NOT NULL,
    "senderId" VARCHAR(50) NOT NULL,
    "senderName" VARCHAR(32) NOT NULL,
    "receiverId" VARCHAR(50) NOT NULL,
    "title" VARCHAR(160) NOT NULL,
    "content" TEXT NOT NULL,
    "gold" INTEGER NOT NULL DEFAULT 0,
    "cash" INTEGER NOT NULL DEFAULT 0,
    "currencies" TEXT NOT NULL,
    "items" TEXT NOT NULL,
    "isRead" BOOLEAN NOT NULL DEFAULT false,
    "readTimestamp" TIMESTAMP(0),
    "isClaim" BOOLEAN NOT NULL DEFAULT false,
    "claimTimestamp" TIMESTAMP(0),
    "isDelete" BOOLEAN NOT NULL DEFAULT false,
    "deleteTimestamp" TIMESTAMP(0),
    "sentTimestamp" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "mail_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "party" (
    "id" SERIAL NOT NULL,
    "shareExp" BOOLEAN NOT NULL,
    "shareItem" BOOLEAN NOT NULL,
    "leaderId" VARCHAR(50) NOT NULL,

    CONSTRAINT "party_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "statistic" (
    "id" SERIAL NOT NULL,
    "userCount" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "statistic_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "storage_reservation" (
    "storageType" SMALLINT NOT NULL,
    "storageOwnerId" VARCHAR(50) NOT NULL,
    "reserverId" VARCHAR(50) NOT NULL,

    CONSTRAINT "storage_reservation_pkey" PRIMARY KEY ("storageType","storageOwnerId")
);

-- CreateTable
CREATE TABLE "storageitem" (
    "id" VARCHAR(50) NOT NULL,
    "idx" INTEGER NOT NULL,
    "storageType" SMALLINT NOT NULL DEFAULT 0,
    "storageOwnerId" VARCHAR(50) NOT NULL,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "amount" INTEGER NOT NULL DEFAULT 0,
    "durability" REAL NOT NULL DEFAULT 0,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "lockRemainsDuration" REAL NOT NULL DEFAULT 0,
    "expireTime" BIGINT NOT NULL DEFAULT 0,
    "randomSeed" INTEGER NOT NULL DEFAULT 0,
    "ammo" INTEGER NOT NULL DEFAULT 0,
    "sockets" TEXT NOT NULL,
    "version" SMALLINT NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "storageitem_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "summonbuffs" (
    "id" VARCHAR(50) NOT NULL,
    "characterId" VARCHAR(50) NOT NULL,
    "buffId" VARCHAR(50) NOT NULL,
    "type" SMALLINT NOT NULL DEFAULT 0,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "buffRemainsDuration" REAL NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "summonbuffs_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "userlogin" (
    "id" VARCHAR(50) NOT NULL,
    "username" VARCHAR(32) NOT NULL,
    "password" VARCHAR(72) NOT NULL,
    "gold" INTEGER NOT NULL DEFAULT 0,
    "cash" INTEGER NOT NULL DEFAULT 0,
    "email" VARCHAR(50) NOT NULL DEFAULT '',
    "isEmailVerified" BOOLEAN NOT NULL DEFAULT false,
    "authType" SMALLINT NOT NULL DEFAULT 1,
    "accessToken" VARCHAR(36) NOT NULL DEFAULT '',
    "userLevel" SMALLINT NOT NULL DEFAULT 0,
    "unbanTime" BIGINT NOT NULL DEFAULT 0,
    "createAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updateAt" TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "userlogin_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE INDEX "character_private_boolean_characterId_idx" ON "character_private_boolean"("characterId");

-- CreateIndex
CREATE INDEX "character_private_float32_characterId_idx" ON "character_private_float32"("characterId");

-- CreateIndex
CREATE INDEX "character_private_int32_characterId_idx" ON "character_private_int32"("characterId");

-- CreateIndex
CREATE INDEX "character_public_boolean_characterId_idx" ON "character_public_boolean"("characterId");

-- CreateIndex
CREATE INDEX "character_public_float32_characterId_idx" ON "character_public_float32"("characterId");

-- CreateIndex
CREATE INDEX "character_public_int32_characterId_idx" ON "character_public_int32"("characterId");

-- CreateIndex
CREATE INDEX "character_server_boolean_characterId_idx" ON "character_server_boolean"("characterId");

-- CreateIndex
CREATE INDEX "character_server_float32_characterId_idx" ON "character_server_float32"("characterId");

-- CreateIndex
CREATE INDEX "character_server_int32_characterId_idx" ON "character_server_int32"("characterId");

-- CreateIndex
CREATE INDEX "characterattribute_characterId_idx" ON "characterattribute"("characterId");

-- CreateIndex
CREATE INDEX "characterbuff_characterId_idx" ON "characterbuff"("characterId");

-- CreateIndex
CREATE INDEX "charactercurrency_characterId_idx" ON "charactercurrency"("characterId");

-- CreateIndex
CREATE INDEX "characterhotkey_characterId_idx" ON "characterhotkey"("characterId");

-- CreateIndex
CREATE INDEX "characterhotkey_hotkeyId_idx" ON "characterhotkey"("hotkeyId");

-- CreateIndex
CREATE INDEX "characteritem_characterId_idx" ON "characteritem"("characterId");

-- CreateIndex
CREATE INDEX "characteritem_idx_idx" ON "characteritem"("idx");

-- CreateIndex
CREATE INDEX "characteritem_inventoryType_idx" ON "characteritem"("inventoryType");

-- CreateIndex
CREATE INDEX "characterquest_characterId_idx" ON "characterquest"("characterId");

-- CreateIndex
CREATE INDEX "characters_factionId_idx" ON "characters"("factionId");

-- CreateIndex
CREATE INDEX "characters_guildId_idx" ON "characters"("guildId");

-- CreateIndex
CREATE INDEX "characters_partyId_idx" ON "characters"("partyId");

-- CreateIndex
CREATE INDEX "characters_userId_idx" ON "characters"("userId");

-- CreateIndex
CREATE INDEX "characterskill_characterId_idx" ON "characterskill"("characterId");

-- CreateIndex
CREATE INDEX "characterskillusage_characterId_idx" ON "characterskillusage"("characterId");

-- CreateIndex
CREATE INDEX "charactersummon_characterId_idx" ON "charactersummon"("characterId");

-- CreateIndex
CREATE INDEX "friend_characterId1_idx" ON "friend"("characterId1");

-- CreateIndex
CREATE INDEX "friend_characterId2_idx" ON "friend"("characterId2");

-- CreateIndex
CREATE INDEX "guild_leaderId_idx" ON "guild"("leaderId");

-- CreateIndex
CREATE INDEX "guildrequest_guildId_idx" ON "guildrequest"("guildId");

-- CreateIndex
CREATE INDEX "guildrequest_requesterId_idx" ON "guildrequest"("requesterId");

-- CreateIndex
CREATE INDEX "mail_eventId_idx" ON "mail"("eventId");

-- CreateIndex
CREATE INDEX "mail_isClaim_idx" ON "mail"("isClaim");

-- CreateIndex
CREATE INDEX "mail_isDelete_idx" ON "mail"("isDelete");

-- CreateIndex
CREATE INDEX "mail_isRead_idx" ON "mail"("isRead");

-- CreateIndex
CREATE INDEX "mail_receiverId_idx" ON "mail"("receiverId");

-- CreateIndex
CREATE INDEX "mail_senderId_idx" ON "mail"("senderId");

-- CreateIndex
CREATE INDEX "mail_senderName_idx" ON "mail"("senderName");

-- CreateIndex
CREATE INDEX "party_leaderId_idx" ON "party"("leaderId");

-- CreateIndex
CREATE INDEX "storage_reservation_reserverId_idx" ON "storage_reservation"("reserverId");

-- CreateIndex
CREATE INDEX "storageitem_idx_idx" ON "storageitem"("idx");

-- CreateIndex
CREATE INDEX "storageitem_storageOwnerId_idx" ON "storageitem"("storageOwnerId");

-- CreateIndex
CREATE INDEX "storageitem_storageType_idx" ON "storageitem"("storageType");

-- CreateIndex
CREATE INDEX "summonbuffs_buffId_idx" ON "summonbuffs"("buffId");

-- CreateIndex
CREATE INDEX "summonbuffs_characterId_idx" ON "summonbuffs"("characterId");

-- CreateIndex
CREATE UNIQUE INDEX "username" ON "userlogin"("username");
