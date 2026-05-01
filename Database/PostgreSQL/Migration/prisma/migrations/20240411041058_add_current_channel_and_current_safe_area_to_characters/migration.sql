-- AlterTable
ALTER TABLE "characters" ADD COLUMN     "current_channel" VARCHAR(50) NOT NULL DEFAULT '',
ADD COLUMN     "current_safe_area" VARCHAR(50) NOT NULL DEFAULT '';
