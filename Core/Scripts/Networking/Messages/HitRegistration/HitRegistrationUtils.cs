using Cysharp.Text;

namespace MultiplayerARPG
{
    public static class HitRegistrationUtils
    {
        public static string MakeValidateId(uint attackerId, int simulateSeed)
        {
            return ZString.Concat(attackerId, "_", simulateSeed);
        }

        public static string MakeHitRegId(byte triggerIndex, byte spreadIndex)
        {
            return ZString.Concat(triggerIndex, "_", spreadIndex);
        }

        public static string MakeHitObjectId(byte triggerIndex, byte spreadIndex, uint objectId)
        {
            return ZString.Concat(triggerIndex, "_", spreadIndex, "_", objectId);
        }
    }
}
