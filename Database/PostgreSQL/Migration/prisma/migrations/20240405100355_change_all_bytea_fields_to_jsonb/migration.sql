/*
  Warnings:

  - Changed the type of `data` on the `character_attributes` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_buffs` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_currencies` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_equip_items` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_hotkeys` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_non_equip_items` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_private_booleans` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_private_float32s` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_private_int32s` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_protected_non_equip_items` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_public_booleans` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_public_float32s` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_public_int32s` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_quests` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_selectable_weapon_sets` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_server_booleans` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_server_float32s` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_server_int32s` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_skill_usages` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_skills` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_summon_buffs` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `character_summons` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `guild_skills` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `storage_buildings` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `storage_guilds` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.
  - Changed the type of `data` on the `storage_users` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.

*/
-- AlterTable
ALTER TABLE "character_attributes" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_buffs" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_currencies" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_equip_items" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_hotkeys" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_non_equip_items" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_private_booleans" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_private_float32s" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_private_int32s" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_protected_non_equip_items" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_public_booleans" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_public_float32s" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_public_int32s" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_quests" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_selectable_weapon_sets" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_server_booleans" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_server_float32s" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_server_int32s" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_skill_usages" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_skills" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_summon_buffs" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "character_summons" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "guild_skills" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "storage_buildings" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "storage_guilds" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;

-- AlterTable
ALTER TABLE "storage_users" DROP COLUMN "data",
ADD COLUMN     "data" JSONB NOT NULL;
