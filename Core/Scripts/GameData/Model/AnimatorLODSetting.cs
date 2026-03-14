namespace MultiplayerARPG
{
    [System.Serializable]
    public struct AnimatorLODSetting : System.IComparable<AnimatorLODSetting>
    {
        public float distance;
        public float framesPerSecond;

        public int CompareTo(AnimatorLODSetting other)
        {
            return distance.CompareTo(other.distance);
        }
    }
}
