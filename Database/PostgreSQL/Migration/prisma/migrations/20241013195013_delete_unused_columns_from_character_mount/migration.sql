/*
  Warnings:

  - You are about to drop the column `current_mp` on the `character_mount` table. All the data in the column will be lost.
  - You are about to drop the column `exp` on the `character_mount` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "character_mount" DROP COLUMN "current_mp",
DROP COLUMN "exp";
