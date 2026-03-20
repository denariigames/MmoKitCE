/*
  Warnings:

  - You are about to drop the column `data_id` on the `character_mount` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "character_mount" DROP COLUMN "data_id",
ADD COLUMN     "source_id" VARCHAR(40) NOT NULL DEFAULT '';
