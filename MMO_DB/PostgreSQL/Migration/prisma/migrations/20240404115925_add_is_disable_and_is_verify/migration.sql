-- AlterTable
ALTER TABLE "users" ADD COLUMN     "is_disable" BOOLEAN NOT NULL DEFAULT false,
ADD COLUMN     "is_verify" BOOLEAN NOT NULL DEFAULT false;
