-- CreateTable
CREATE TABLE "character_appearances" (
    "id" VARCHAR(40) NOT NULL,
    "data" JSONB NOT NULL,
    "update_time" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "character_appearances_pkey" PRIMARY KEY ("id")
);
