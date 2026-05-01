-- CreateTable
CREATE TABLE "server_statistic" (
    "id" SERIAL NOT NULL,
    "user_count" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "server_statistic_pkey" PRIMARY KEY ("id")
);
