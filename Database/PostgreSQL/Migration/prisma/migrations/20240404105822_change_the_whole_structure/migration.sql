/*
  Warnings:

  - The primary key for the `buildings` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - You are about to drop the column `createAt` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `creatorId` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `creatorName` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `currentHp` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `entityId` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `extraData` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `isLocked` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `isSceneObject` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `lockPassword` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `mapName` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `parentId` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `positionX` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `positionY` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `positionZ` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `remainsLifeTime` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `rotationX` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `rotationY` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `rotationZ` on the `buildings` table. All the data in the column will be lost.
  - You are about to drop the column `updateAt` on the `buildings` table. All the data in the column will be lost.
  - You are about to alter the column `id` on the `buildings` table. The data in that column could be lost. The data in that column will be cast from `VarChar(50)` to `VarChar(40)`.
  - The primary key for the `character_pk` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - You are about to drop the column `consecutivePkKills` on the `character_pk` table. All the data in the column will be lost.
  - You are about to drop the column `highestConsecutivePkKills` on the `character_pk` table. All the data in the column will be lost.
  - You are about to drop the column `highestPkPoint` on the `character_pk` table. All the data in the column will be lost.
  - You are about to drop the column `isPkOn` on the `character_pk` table. All the data in the column will be lost.
  - You are about to drop the column `lastPkOnTime` on the `character_pk` table. All the data in the column will be lost.
  - You are about to drop the column `pkPoint` on the `character_pk` table. All the data in the column will be lost.
  - You are about to alter the column `id` on the `character_pk` table. The data in that column could be lost. The data in that column will be cast from `VarChar(255)` to `VarChar(40)`.
  - The primary key for the `characters` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - You are about to drop the column `characterName` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `createAt` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentFood` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentHp` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentMapName` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentMp` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentPositionX` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentPositionY` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentPositionZ` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentRotationX` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentRotationY` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentRotationZ` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentStamina` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `currentWater` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `dataId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `entityId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `equipWeaponSet` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `factionId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `frameDataId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `guildId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `guildRole` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `iconDataId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `lastDeadTime` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `mountDataId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `partyId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `respawnMapName` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `respawnPositionX` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `respawnPositionY` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `respawnPositionZ` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `sharedGuildExp` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `skillPoint` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `statPoint` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `titleDataId` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `unmuteTime` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `updateAt` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `userId` on the `characters` table. All the data in the column will be lost.
  - You are about to alter the column `id` on the `characters` table. The data in that column could be lost. The data in that column will be cast from `VarChar(50)` to `VarChar(40)`.
  - You are about to drop the `character_private_boolean` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_private_float32` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_private_int32` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_public_boolean` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_public_float32` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_public_int32` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_server_boolean` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_server_float32` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `character_server_int32` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characterattribute` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characterbuff` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `charactercurrency` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characterhotkey` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characteritem` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characterquest` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characterskill` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `characterskillusage` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `charactersummon` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `friend` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `guild` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `guildrequest` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `guildrole` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `guildskill` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `mail` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `party` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `statistic` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `storage_reservation` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `storageitem` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `summonbuffs` table. If the table is not empty, all the data it contains will be lost.
  - You are about to drop the `userlogin` table. If the table is not empty, all the data it contains will be lost.
  - A unique constraint covering the columns `[character_name]` on the table `characters` will be added. If there are existing duplicate values, this will fail.
  - Added the required column `extra_data` to the `buildings` table without a default value. This is not possible if the table is not empty.
  - Added the required column `map_name` to the `buildings` table without a default value. This is not possible if the table is not empty.
  - Added the required column `character_name` to the `characters` table without a default value. This is not possible if the table is not empty.
  - Added the required column `user_id` to the `characters` table without a default value. This is not possible if the table is not empty.

*/
-- DropIndex
DROP INDEX "characters_factionId_idx";

-- DropIndex
DROP INDEX "characters_guildId_idx";

-- DropIndex
DROP INDEX "characters_partyId_idx";

-- DropIndex
DROP INDEX "characters_userId_idx";

-- AlterTable
ALTER TABLE "buildings" DROP CONSTRAINT "buildings_pkey",
DROP COLUMN "createAt",
DROP COLUMN "creatorId",
DROP COLUMN "creatorName",
DROP COLUMN "currentHp",
DROP COLUMN "entityId",
DROP COLUMN "extraData",
DROP COLUMN "isLocked",
DROP COLUMN "isSceneObject",
DROP COLUMN "lockPassword",
DROP COLUMN "mapName",
DROP COLUMN "parentId",
DROP COLUMN "positionX",
DROP COLUMN "positionY",
DROP COLUMN "positionZ",
DROP COLUMN "remainsLifeTime",
DROP COLUMN "rotationX",
DROP COLUMN "rotationY",
DROP COLUMN "rotationZ",
DROP COLUMN "updateAt",
ADD COLUMN     "create_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN     "creator_id" VARCHAR(40) NOT NULL DEFAULT '',
ADD COLUMN     "creator_name" VARCHAR(32) NOT NULL DEFAULT '',
ADD COLUMN     "current_hp" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "entity_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "extra_data" TEXT NOT NULL,
ADD COLUMN     "is_locked" BOOLEAN NOT NULL DEFAULT false,
ADD COLUMN     "is_scene_object" BOOLEAN NOT NULL DEFAULT false,
ADD COLUMN     "lock_password" VARCHAR(6) NOT NULL DEFAULT '',
ADD COLUMN     "map_name" VARCHAR(50) NOT NULL,
ADD COLUMN     "parent_id" VARCHAR(40) NOT NULL DEFAULT '',
ADD COLUMN     "position_x" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "position_y" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "position_z" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "remains_lifetime" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "rotation_x" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "rotation_y" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "rotation_z" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
ALTER COLUMN "id" SET DATA TYPE VARCHAR(40),
ADD CONSTRAINT "buildings_pkey" PRIMARY KEY ("id");

-- AlterTable
ALTER TABLE "character_pk" DROP CONSTRAINT "character_pk_pkey",
DROP COLUMN "consecutivePkKills",
DROP COLUMN "highestConsecutivePkKills",
DROP COLUMN "highestPkPoint",
DROP COLUMN "isPkOn",
DROP COLUMN "lastPkOnTime",
DROP COLUMN "pkPoint",
ADD COLUMN     "consecutive_pk_kills" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "highest_consecutive_pk_kills" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "highest_pk_point" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "is_pk_on" BOOLEAN NOT NULL DEFAULT false,
ADD COLUMN     "last_pk_on_time" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "pk_point" INTEGER NOT NULL DEFAULT 0,
ALTER COLUMN "id" SET DATA TYPE VARCHAR(40),
ADD CONSTRAINT "character_pk_pkey" PRIMARY KEY ("id");

-- AlterTable
ALTER TABLE "characters" DROP CONSTRAINT "characters_pkey",
DROP COLUMN "characterName",
DROP COLUMN "createAt",
DROP COLUMN "currentFood",
DROP COLUMN "currentHp",
DROP COLUMN "currentMapName",
DROP COLUMN "currentMp",
DROP COLUMN "currentPositionX",
DROP COLUMN "currentPositionY",
DROP COLUMN "currentPositionZ",
DROP COLUMN "currentRotationX",
DROP COLUMN "currentRotationY",
DROP COLUMN "currentRotationZ",
DROP COLUMN "currentStamina",
DROP COLUMN "currentWater",
DROP COLUMN "dataId",
DROP COLUMN "entityId",
DROP COLUMN "equipWeaponSet",
DROP COLUMN "factionId",
DROP COLUMN "frameDataId",
DROP COLUMN "guildId",
DROP COLUMN "guildRole",
DROP COLUMN "iconDataId",
DROP COLUMN "lastDeadTime",
DROP COLUMN "mountDataId",
DROP COLUMN "partyId",
DROP COLUMN "respawnMapName",
DROP COLUMN "respawnPositionX",
DROP COLUMN "respawnPositionY",
DROP COLUMN "respawnPositionZ",
DROP COLUMN "sharedGuildExp",
DROP COLUMN "skillPoint",
DROP COLUMN "statPoint",
DROP COLUMN "titleDataId",
DROP COLUMN "unmuteTime",
DROP COLUMN "updateAt",
DROP COLUMN "userId",
ADD COLUMN     "character_name" VARCHAR(32) NOT NULL,
ADD COLUMN     "create_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN     "current_food" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "current_hp" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "current_map_name" VARCHAR(50) NOT NULL DEFAULT '',
ADD COLUMN     "current_mp" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "current_position_x" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "current_position_y" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "current_position_z" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "current_rotation_x" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "current_rotation_y" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "current_rotation_z" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "current_stamina" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "current_water" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "entity_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "equip_weapon_set" SMALLINT NOT NULL DEFAULT 0,
ADD COLUMN     "faction_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "frame_data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "guild_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "guild_role" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "icon_data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "last_dead_time" BIGINT NOT NULL DEFAULT 0,
ADD COLUMN     "mount_data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "party_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "pet_data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "reputation" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "respawn_map_name" VARCHAR(50) NOT NULL DEFAULT '',
ADD COLUMN     "respawn_position_x" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "respawn_position_y" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "respawn_position_z" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "shared_guild_exp" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "skill_point" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "stat_point" REAL NOT NULL DEFAULT 0,
ADD COLUMN     "title_data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "unmute_time" BIGINT NOT NULL DEFAULT 0,
ADD COLUMN     "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN     "user_id" VARCHAR(40) NOT NULL,
ALTER COLUMN "id" SET DATA TYPE VARCHAR(40),
ADD CONSTRAINT "characters_pkey" PRIMARY KEY ("id");

-- DropTable
DROP TABLE "character_private_boolean";

-- DropTable
DROP TABLE "character_private_float32";

-- DropTable
DROP TABLE "character_private_int32";

-- DropTable
DROP TABLE "character_public_boolean";

-- DropTable
DROP TABLE "character_public_float32";

-- DropTable
DROP TABLE "character_public_int32";

-- DropTable
DROP TABLE "character_server_boolean";

-- DropTable
DROP TABLE "character_server_float32";

-- DropTable
DROP TABLE "character_server_int32";

-- DropTable
DROP TABLE "characterattribute";

-- DropTable
DROP TABLE "characterbuff";

-- DropTable
DROP TABLE "charactercurrency";

-- DropTable
DROP TABLE "characterhotkey";

-- DropTable
DROP TABLE "characteritem";

-- DropTable
DROP TABLE "characterquest";

-- DropTable
DROP TABLE "characterskill";

-- DropTable
DROP TABLE "characterskillusage";

-- DropTable
DROP TABLE "charactersummon";

-- DropTable
DROP TABLE "friend";

-- DropTable
DROP TABLE "guild";

-- DropTable
DROP TABLE "guildrequest";

-- DropTable
DROP TABLE "guildrole";

-- DropTable
DROP TABLE "guildskill";

-- DropTable
DROP TABLE "mail";

-- DropTable
DROP TABLE "party";

-- DropTable
DROP TABLE "statistic";

-- DropTable
DROP TABLE "storage_reservation";

-- DropTable
DROP TABLE "storageitem";

-- DropTable
DROP TABLE "summonbuffs";

-- DropTable
DROP TABLE "userlogin";

-- CreateTable
CREATE TABLE "users" (
    "id" VARCHAR(40) NOT NULL,
    "username" VARCHAR(128),
    "display_name" VARCHAR(255),
    "avatar_url" VARCHAR(512),
    "email" VARCHAR(255),
    "facebook_id" VARCHAR(128),
    "google_id" VARCHAR(128),
    "apple_id" VARCHAR(128),
    "steam_id" VARCHAR(128),
    "custom_id" VARCHAR(128),
    "password" VARCHAR(128),
    "create_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "verify_time" TIMESTAMPTZ NOT NULL DEFAULT '1970-01-01 00:00:00 +00:00',
    "disable_time" TIMESTAMPTZ NOT NULL DEFAULT '1970-01-01 00:00:00 +00:00',

    CONSTRAINT "users_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_attributes" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_attributes_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_buffs" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_buffs_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_currencies" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_currencies_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_hotkeys" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_hotkeys_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_equip_items" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_equip_items_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_selectable_weapon_sets" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_selectable_weapon_sets_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_non_equip_items" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_non_equip_items_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_protected_non_equip_items" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_protected_non_equip_items_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_quests" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_quests_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_skills" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_skills_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_skill_usages" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_skill_usages_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_summons" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_summons_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_private_booleans" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_private_booleans_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_private_float32s" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_private_float32s_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_private_int32s" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_private_int32s_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_public_booleans" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_public_booleans_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_public_float32s" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_public_float32s_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_public_int32s" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_public_int32s_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_server_booleans" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_server_booleans_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_server_float32s" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_server_float32s_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_server_int32s" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_server_int32s_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_summon_buffs" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_summon_buffs_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "character_combat_stats" (
    "id" VARCHAR(40) NOT NULL,
    "player_kills" INTEGER NOT NULL DEFAULT 0,
    "player_headshots" INTEGER NOT NULL DEFAULT 0,
    "player_assists" INTEGER NOT NULL DEFAULT 0,
    "zombie_kills" INTEGER NOT NULL DEFAULT 0,
    "zombie_headshots" INTEGER NOT NULL DEFAULT 0,
    "zombie_assists" INTEGER NOT NULL DEFAULT 0,
    "highest_player_kills" INTEGER NOT NULL DEFAULT 0,
    "highest_player_headshots" INTEGER NOT NULL DEFAULT 0,
    "highest_player_assists" INTEGER NOT NULL DEFAULT 0,
    "highest_zombie_kills" INTEGER NOT NULL DEFAULT 0,
    "highest_zombie_headshots" INTEGER NOT NULL DEFAULT 0,
    "highest_zombie_assists" INTEGER NOT NULL DEFAULT 0,
    "player_killed" INTEGER NOT NULL DEFAULT 0,
    "player_killed_headshots" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "character_combat_stats_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "friends" (
    "character_id_1" VARCHAR(40) NOT NULL,
    "character_id_2" VARCHAR(40) NOT NULL,
    "state" SMALLINT NOT NULL DEFAULT 0,
    "create_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "friends_pkey" PRIMARY KEY ("character_id_1","character_id_2")
);

-- CreateTable
CREATE TABLE "guilds" (
    "id" SERIAL NOT NULL,
    "guild_name" VARCHAR(32) NOT NULL,
    "leader_id" VARCHAR(40) NOT NULL,
    "level" INTEGER NOT NULL DEFAULT 1,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "skill_point" INTEGER NOT NULL DEFAULT 0,
    "guild_message" VARCHAR(40) NOT NULL,
    "guild_message_2" VARCHAR(40) NOT NULL,
    "gold" INTEGER NOT NULL DEFAULT 0,
    "score" INTEGER NOT NULL DEFAULT 0,
    "options" TEXT NOT NULL,
    "auto_accept_requests" BOOLEAN NOT NULL DEFAULT false,
    "rank" INTEGER NOT NULL DEFAULT 0,
    "current_members" INTEGER NOT NULL DEFAULT 0,
    "max_members" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "guilds_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "guild_requests" (
    "id" INTEGER NOT NULL,
    "requester_id" VARCHAR(40) NOT NULL,
    "create_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "guild_requests_pkey" PRIMARY KEY ("id","requester_id")
);

-- CreateTable
CREATE TABLE "guild_roles" (
    "id" INTEGER NOT NULL,
    "role" INTEGER NOT NULL,
    "name" VARCHAR(50) NOT NULL,
    "can_invite" BOOLEAN NOT NULL DEFAULT false,
    "can_kick" BOOLEAN NOT NULL DEFAULT false,
    "can_use_storage" BOOLEAN NOT NULL DEFAULT false,
    "share_exp_percentage" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "guild_roles_pkey" PRIMARY KEY ("id","role")
);

-- CreateTable
CREATE TABLE "guild_skills" (
    "id" INTEGER NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "guild_skills_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "mails" (
    "id" BIGSERIAL NOT NULL,
    "event_id" VARCHAR(50) NOT NULL DEFAULT '',
    "sender_id" VARCHAR(50) NOT NULL DEFAULT '',
    "sender_name" VARCHAR(32) NOT NULL DEFAULT '',
    "receiver_id" VARCHAR(50) NOT NULL DEFAULT '',
    "title" VARCHAR(50) NOT NULL DEFAULT '',
    "content" TEXT NOT NULL DEFAULT '',
    "gold" INTEGER NOT NULL DEFAULT 0,
    "cash" INTEGER NOT NULL DEFAULT 0,
    "currencies" TEXT NOT NULL DEFAULT '',
    "items" TEXT NOT NULL DEFAULT '',
    "is_read" BOOLEAN NOT NULL DEFAULT false,
    "read_time" TIMESTAMPTZ,
    "is_claim" BOOLEAN NOT NULL DEFAULT false,
    "claim_time" TIMESTAMPTZ,
    "is_delete" BOOLEAN NOT NULL DEFAULT false,
    "delete_time" TIMESTAMPTZ,
    "sent_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "mails_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "parties" (
    "id" SERIAL NOT NULL,
    "share_exp" BOOLEAN NOT NULL DEFAULT false,
    "share_item" BOOLEAN NOT NULL DEFAULT false,
    "leader_id" VARCHAR(40) NOT NULL,

    CONSTRAINT "parties_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "storage_guilds" (
    "id" INTEGER NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "storage_guilds_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "storage_users" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "storage_users_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "storage_buildings" (
    "id" VARCHAR(40) NOT NULL,
    "data" BYTEA NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "storage_buildings_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE UNIQUE INDEX "users_username_key" ON "users"("username");

-- CreateIndex
CREATE UNIQUE INDEX "users_email_key" ON "users"("email");

-- CreateIndex
CREATE UNIQUE INDEX "users_facebook_id_key" ON "users"("facebook_id");

-- CreateIndex
CREATE UNIQUE INDEX "users_google_id_key" ON "users"("google_id");

-- CreateIndex
CREATE UNIQUE INDEX "users_apple_id_key" ON "users"("apple_id");

-- CreateIndex
CREATE UNIQUE INDEX "users_steam_id_key" ON "users"("steam_id");

-- CreateIndex
CREATE UNIQUE INDEX "users_custom_id_key" ON "users"("custom_id");

-- CreateIndex
CREATE UNIQUE INDEX "guilds_guild_name_key" ON "guilds"("guild_name");

-- CreateIndex
CREATE UNIQUE INDEX "guild_roles_name_key" ON "guild_roles"("name");

-- CreateIndex
CREATE UNIQUE INDEX "characters_character_name_key" ON "characters"("character_name");
