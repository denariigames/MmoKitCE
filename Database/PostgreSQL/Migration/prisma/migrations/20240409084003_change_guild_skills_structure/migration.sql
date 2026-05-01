/*
  Warnings:

  - The primary key for the `guild_skills` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - You are about to drop the column `data` on the `guild_skills` table. All the data in the column will be lost.
  - Added the required column `data_id` to the `guild_skills` table without a default value. This is not possible if the table is not empty.
  - Added the required column `level` to the `guild_skills` table without a default value. This is not possible if the table is not empty.

*/
-- AlterTable
ALTER TABLE "guild_skills" DROP CONSTRAINT "guild_skills_pkey",
DROP COLUMN "data",
ADD COLUMN     "create_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN     "data_id" INTEGER NOT NULL,
ADD COLUMN     "level" INTEGER NOT NULL,
ADD CONSTRAINT "guild_skills_pkey" PRIMARY KEY ("id", "data_id");
