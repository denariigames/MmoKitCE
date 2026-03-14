namespace MultiplayerARPG
{
    public class UserUsingStorageData
    {
        public StorageId Id { get; set; }
        public bool RequireEntity { get; set; }
        public IActivatableEntity Entity { get; set; }

        public string GetId()
        {
            return Id.GetId();
        }

        public override int GetHashCode()
        {
            return GetId().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return GetId();
        }
    }
}
