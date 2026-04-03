using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public static partial class GenericUtils
{
    private static System.Random randomizer = new System.Random();
    private static System.Random seedRandomizer = new System.Random(System.DateTime.Now.Millisecond);

    public static string GetUniqueId()
    {
        return System.Guid.NewGuid().ToString("N");
    }

    public static string GetMD5(this string text)
    {
        // byte array representation of that string
        byte[] encodedPassword = new UTF8Encoding().GetBytes(text);

        // need MD5 to calculate the hash
        byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

        // string representation (similar to UNIX format)
        return System.BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }

    public static int GenerateHashId(this string id)
    {
        if (string.IsNullOrEmpty(id))
            return 0;

        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < id.Length && id[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ id[i];
                if (i == id.Length - 1 || id[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ id[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    public static void Shuffle<T>(this IList<T> list, System.Random random)
    {
        if (list == null || list.Count <= 1)
            return;
        int tempRandomIndex;
        T tempEntry;
        for (int i = 0; i < list.Count - 1; ++i)
        {
            tempRandomIndex = random.Next(i, list.Count);
            tempEntry = list[i];
            list[i] = list[tempRandomIndex];
            list[tempRandomIndex] = tempEntry;
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        randomizer.Next();
        Shuffle(list, randomizer);
    }

    public static int Increase(this int a, int b)
    {
        try
        {
            checked
            {
                return a + b;
            }
        }
        catch (System.OverflowException)
        {
            return int.MaxValue;
        }
    }

    public static float RandomFloat(this System.Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public static float RandomFloat(int seed, float min, float max)
    {
        return new System.Random(seed).RandomFloat(min, max);
    }

    public static float RandomFloat(float min, float max)
    {
        return new System.Random(seedRandomizer.Next()).RandomFloat(min, max);
    }

    public static int RandomInt(this System.Random random, int min, int max)
    {
        return random.Next(min, max);
    }

    public static int RandomInt(int seed, int min, int max)
    {
        return new System.Random(seed).RandomInt(min, max);
    }

    public static int RandomInt(int min, int max)
    {
        return new System.Random(seedRandomizer.Next()).RandomInt(min, max);
    }

    public static System.DateTime GetStartDateTime()
    {
        return new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    }

    public static System.DateTime GetDateTimeByMilliseconds(long milliseconds)
    {
        return GetStartDateTime().AddMilliseconds(milliseconds);
    }

    public static System.DateTime GetDateTimeBySeconds(long seconds)
    {
        return GetStartDateTime().AddSeconds(seconds);
    }

    public static System.DateTime StartOfWeek(this System.DateTime dateTime, System.DayOfWeek startOfWeek)
    {
        int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }
}
