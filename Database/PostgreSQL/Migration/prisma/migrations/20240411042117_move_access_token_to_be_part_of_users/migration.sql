/*
  Warnings:

  - You are about to drop the column `access_token` on the `user_accesses` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "user_accesses" DROP COLUMN "access_token";

-- AlterTable
ALTER TABLE "users" ADD COLUMN     "access_token" VARCHAR(128);
