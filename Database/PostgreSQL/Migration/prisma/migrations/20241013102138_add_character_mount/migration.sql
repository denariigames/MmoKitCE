-- CreateTable
CREATE TABLE "character_mount" (
    "id" VARCHAR(40) NOT NULL,
    "type" SMALLINT NOT NULL DEFAULT 0,
    "dataId" INTEGER NOT NULL DEFAULT 0,
    "level" INTEGER NOT NULL DEFAULT 1,
    "exp" INTEGER NOT NULL DEFAULT 0,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_mount_pkey" PRIMARY KEY ("id")
);
