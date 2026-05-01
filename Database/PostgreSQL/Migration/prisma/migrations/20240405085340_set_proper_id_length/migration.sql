/*
  Warnings:

  - You are about to alter the column `sender_id` on the `mails` table. The data in that column could be lost. The data in that column will be cast from `VarChar(50)` to `VarChar(40)`.
  - You are about to alter the column `receiver_id` on the `mails` table. The data in that column could be lost. The data in that column will be cast from `VarChar(50)` to `VarChar(40)`.

*/
-- AlterTable
ALTER TABLE "mails" ALTER COLUMN "sender_id" SET DATA TYPE VARCHAR(40),
ALTER COLUMN "receiver_id" SET DATA TYPE VARCHAR(40);
