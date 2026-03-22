// cf scalabilty: #10

using LiteNetLib.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class SerializationABBenchmarkMenu
    {
        [MenuItem("MMORPG Kit/MmoKitCE/Networking/Run Serialization A-B Benchmark")]
        public static void RunSerializationABBenchmark()
        {
            bool originalQuantizedMovementVectors = EntityMovementFunctions.UseQuantizedMovementVectors;
            bool originalQuantizedMovementVectorsSetting = NetworkSerializationSettings.UseQuantizedMovementVectors;
            float originalQuantizedPrecision = EntityMovementFunctions.QuantizedMovementPrecision;

            try
            {
                BenchmarkResult off = Measure(useQuantizedMovementVectors: false, quantizedPrecision: originalQuantizedPrecision);
                BenchmarkResult on = Measure(useQuantizedMovementVectors: true, quantizedPrecision: originalQuantizedPrecision);

                int delta = off.TotalBytes - on.TotalBytes;
                float savingsPercent = off.TotalBytes > 0 ? (delta / (float)off.TotalBytes) * 100f : 0f;

                Debug.Log(
                    $"[Serialization A/B] OFF={off.TotalBytes} bytes, ON={on.TotalBytes} bytes, " +
                    $"delta={delta} bytes ({savingsPercent:F2}% saved)\n" +
                    $"[Serialization A/B] Details OFF: {off.Detail}\n" +
                    $"[Serialization A/B] Details ON: {on.Detail}");
            }
            finally
            {
                NetworkSerializationSettings.UseQuantizedMovementVectors = originalQuantizedMovementVectorsSetting;
                EntityMovementFunctions.UseQuantizedMovementVectors = originalQuantizedMovementVectors;
                EntityMovementFunctions.QuantizedMovementPrecision = originalQuantizedPrecision;
            }
        }

        private static BenchmarkResult Measure(bool useQuantizedMovementVectors, float quantizedPrecision)
        {
            NetworkSerializationSettings.UseQuantizedMovementVectors = useQuantizedMovementVectors;

            EntityMovementFunctions.UseQuantizedMovementVectors = useQuantizedMovementVectors;
            EntityMovementFunctions.QuantizedMovementPrecision = Mathf.Max(1f, quantizedPrecision);

            const int socialCount = 200;
            const int guildCount = 100;
            const int mailCount = 100;
            const int idListCount = 300;
            const int rewardCount = 80;
            const int formattedArgsCount = 30;

            var socialList = MakeList<SocialCharacterData>(socialCount);
            var guildList = MakeList<GuildListEntry>(guildCount);
            var mailList = MakeList<MailListEntry>(mailCount);
            var rewardList = MakeList<RewardedItem>(rewardCount);
            var idList = MakeIntList(idListCount);
            var formattedArgs = MakeStringArray(formattedArgsCount);
            var movementVectors3D = MakeVector3Array(1000);
            var movementVectors2D = MakeVector2Array(1000);

            int total = 0;
            total += GetSerializedSize(new ResponseGetFriendsMessage { message = UITextKeys.NONE, friends = socialList });
            total += GetSerializedSize(new ResponseGetFriendRequestsMessage { message = UITextKeys.NONE, friendRequests = socialList });
            total += GetSerializedSize(new ResponseSocialCharacterListMessage { message = UITextKeys.NONE, characters = socialList });
            total += GetSerializedSize(new ResponseGetGuildRequestsMessage { message = UITextKeys.NONE, guildRequests = socialList });
            total += GetSerializedSize(new UpdateSocialMembersMessage { id = 1, members = socialList });
            total += GetSerializedSize(new ResponseFindGuildsMessage { message = UITextKeys.NONE, guilds = guildList });
            total += GetSerializedSize(new ResponseMailListMessage { onlyNewMails = false, mails = mailList });
            total += GetSerializedSize(new ResponseCashShopInfoMessage { message = UITextKeys.NONE, cash = 1000, cashShopItemIds = idList });
            total += GetSerializedSize(new ResponseCashPackageInfoMessage { message = UITextKeys.NONE, cash = 1000, cashPackageIds = idList });
            total += GetSerializedSize(new ResponseCashShopBuyMessage { message = UITextKeys.NONE, dataId = 1, rewardGold = 500, rewardItems = rewardList });
            total += GetSerializedSize(new ResponseGachaInfoMessage { message = UITextKeys.NONE, cash = 1000, gachaIds = idList });
            total += GetSerializedSize(new ResponseOpenGachaMessage { message = UITextKeys.NONE, dataId = 1, rewardItems = rewardList });
            total += GetSerializedSize(new FormattedGameMessage { format = default, args = formattedArgs });
            total += GetMovementVectorPayloadSize(movementVectors3D, movementVectors2D);

            string detail =
                $"social={socialCount}, guild={guildCount}, mail={mailCount}, ids={idListCount}, " +
                $"rewards={rewardCount}, fmtArgs={formattedArgsCount}, move3D={movementVectors3D.Length}, move2D={movementVectors2D.Length}, " +
                $"quantizedPrecision={EntityMovementFunctions.QuantizedMovementPrecision:F2}";

            return new BenchmarkResult(total, detail);
        }

        private static List<T> MakeList<T>(int count) where T : struct
        {
            var result = new List<T>(count);
            for (int i = 0; i < count; ++i)
                result.Add(default);
            return result;
        }

        private static List<int> MakeIntList(int count)
        {
            var result = new List<int>(count);
            for (int i = 0; i < count; ++i)
                result.Add(i + 1);
            return result;
        }

        private static string[] MakeStringArray(int count)
        {
            var result = new string[count];
            for (int i = 0; i < count; ++i)
                result[i] = "arg_" + i;
            return result;
        }

        private static Vector3[] MakeVector3Array(int count)
        {
            var result = new Vector3[count];
            for (int i = 0; i < count; ++i)
                result[i] = new Vector3(i * 0.13f, (i % 40) * 0.08f, (i % 70) * 0.11f);
            return result;
        }

        private static Vector2[] MakeVector2Array(int count)
        {
            var result = new Vector2[count];
            for (int i = 0; i < count; ++i)
                result[i] = new Vector2(i * 0.09f, (i % 50) * 0.07f);
            return result;
        }

        private static int GetMovementVectorPayloadSize(Vector3[] vectors3D, Vector2[] vectors2D)
        {
            var writer = new NetDataWriter();
            for (int i = 0; i < vectors3D.Length; ++i)
            {
                if (EntityMovementFunctions.UseQuantizedMovementVectors)
                    writer.PutQuantizedVector3(vectors3D[i], EntityMovementFunctions.QuantizedMovementPrecision);
                else
                    writer.PutVector3(vectors3D[i]);
            }
            for (int i = 0; i < vectors2D.Length; ++i)
            {
                if (EntityMovementFunctions.UseQuantizedMovementVectors)
                    writer.PutQuantizedVector2(vectors2D[i], EntityMovementFunctions.QuantizedMovementPrecision);
                else
                    writer.PutVector2(vectors2D[i]);
            }
            return writer.Length;
        }

        private static int GetSerializedSize<T>(T message) where T : INetSerializable
        {
            var writer = new NetDataWriter();
            message.Serialize(writer);
            return writer.Length;
        }

        private readonly struct BenchmarkResult
        {
            public readonly int TotalBytes;
            public readonly string Detail;

            public BenchmarkResult(int totalBytes, string detail)
            {
                TotalBytes = totalBytes;
                Detail = detail;
            }
        }
    }
}
