using System.Security.Cryptography;
using System.Text;

namespace MultiplayerARPG.MMO
{
    /// <summary>
    /// The default one use nanoid to generate ID, use MD5 (with salt setting) I recommend you to create new database user login manager because MD5 is considered insecure nowsaday
    /// </summary>
    public class DefaultDatabaseUserLogin : IDatabaseUserLogin
    {
        private string _passwordSaltPrefix = string.Empty;
        private string _passwordSaltPostfix = string.Empty;

        public DefaultDatabaseUserLogin(DefaultDatabaseUserLoginConfig config)
        {
            _passwordSaltPrefix = config.PasswordSaltPrefix;
            _passwordSaltPostfix = config.PasswordSaltPostfix;
        }

        public string GenerateNewId()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        public string GetHashedPassword(string password)
        {
            return GetMD5(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return GetMD5(password).Equals(hashedPassword);
        }

        private string GetMD5(string text)
        {
            if (!string.IsNullOrEmpty(_passwordSaltPrefix))
                text = _passwordSaltPrefix + text;
            if (!string.IsNullOrEmpty(_passwordSaltPostfix))
                text = text + _passwordSaltPostfix;

            // byte array representation of that string
            byte[] encodedPassword = new UTF8Encoding().GetBytes(text);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            return System.BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
    }
}
