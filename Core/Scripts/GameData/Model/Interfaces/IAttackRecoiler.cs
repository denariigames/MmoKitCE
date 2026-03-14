namespace MultiplayerARPG
{
    public interface IAttackRecoiler
    {
        public float DefaultRecoilDuration { get; }
        public void PlayRecoiling();
    }
}
