/*
  Warnings:

  - You are about to drop the `storage_reservation` table. If the table is not empty, all the data it contains will be lost.

*/
-- DropTable
DROP TABLE "storage_reservation";

-- CreateTable
CREATE TABLE "storage_reservation_guilds" (
    "id" INTEGER NOT NULL,
    "reserver_id" VARCHAR(40) NOT NULL,

    CONSTRAINT "storage_reservation_guilds_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "storage_reservation_users" (
    "id" VARCHAR(40) NOT NULL,
    "reserver_id" VARCHAR(40) NOT NULL,

    CONSTRAINT "storage_reservation_users_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "storage_reservation_buildings" (
    "id" VARCHAR(40) NOT NULL,
    "reserver_id" VARCHAR(40) NOT NULL,

    CONSTRAINT "storage_reservation_buildings_pkey" PRIMARY KEY ("id")
);
