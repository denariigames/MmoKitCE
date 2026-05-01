-- CreateTable
CREATE TABLE "user_currencies" (
    "id" VARCHAR(40) NOT NULL,
    "gold" INTEGER NOT NULL DEFAULT 0,
    "cash" INTEGER NOT NULL DEFAULT 0,

    CONSTRAINT "user_currencies_pkey" PRIMARY KEY ("id")
);
