/*
  Warnings:

  - You are about to drop the column `mount_data_id` on the `characters` table. All the data in the column will be lost.
  - You are about to drop the column `pet_data_id` on the `characters` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "characters" DROP COLUMN "mount_data_id",
DROP COLUMN "pet_data_id";
