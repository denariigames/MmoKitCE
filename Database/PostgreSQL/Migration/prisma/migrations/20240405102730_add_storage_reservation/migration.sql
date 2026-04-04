-- CreateTable
CREATE TABLE "storage_reservation" (
    "storage_type" SMALLINT NOT NULL DEFAULT 0,
    "storage_owner_id" VARCHAR(40) NOT NULL,
    "reserver_id" VARCHAR(40) NOT NULL,

    CONSTRAINT "storage_reservation_pkey" PRIMARY KEY ("storage_type","storage_owner_id")
);
