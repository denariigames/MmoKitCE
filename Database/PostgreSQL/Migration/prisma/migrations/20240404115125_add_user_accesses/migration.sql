/*
  Warnings:

  - You are about to drop the column `access_token` on the `users` table. All the data in the column will be lost.
  - You are about to drop the column `level` on the `users` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "users" DROP COLUMN "access_token",
DROP COLUMN "level";

-- CreateTable
CREATE TABLE "user_accesses" (
    "id" VARCHAR(40) NOT NULL,
    "level" INTEGER NOT NULL DEFAULT 0,
    "access_token" VARCHAR(128),
    "unban_time" BIGINT NOT NULL DEFAULT 0,

    CONSTRAINT "user_accesses_pkey" PRIMARY KEY ("id")
);
