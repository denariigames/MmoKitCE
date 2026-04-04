/*
  Warnings:

  - You are about to drop the column `dataId` on the `character_mount` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "character_mount" DROP COLUMN "dataId",
ADD COLUMN     "current_hp" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "current_mp" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "data_id" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN     "mount_remains_duration" REAL NOT NULL DEFAULT 0;
