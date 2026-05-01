namespace MultiplayerARPG.MMO
{
    public interface IDatabaseUserLogin
    {
        string GetHashedPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateNewId();
    }
}
