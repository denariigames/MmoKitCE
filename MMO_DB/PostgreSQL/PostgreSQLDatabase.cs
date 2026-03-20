#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase : BaseDatabase
    {
        public static readonly string LogTag = nameof(PostgreSQLDatabase);

        private string _address = "127.0.0.1";
        private int _port = 5432;
        private string _username = "postgres";
        private string _password = "localdb";
        private string _dbName = "mmorpg_kit";
        private string _connectionString = "";
        private NpgsqlDataSource _dataSource;

        private event DbGetCharacterDelegate onGetCharacter = null;
        private event DbCreateCharacterDelegate<NpgsqlConnection, NpgsqlTransaction> onCreateCharacter = null;
        private event DbUpdateCharacterDelegate<NpgsqlConnection, NpgsqlTransaction> onUpdateCharacter = null;
        private event DbDeleteCharacterDelegate<NpgsqlConnection, NpgsqlTransaction> onDeleteCharacter = null;

        public override void Initialize()
        {
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/pgsqlConfig.json";
            PostgreSQLConfig config = new PostgreSQLConfig()
            {
                pgAddress = _address,
                pgPort = _port,
                pgUsername = _username,
                pgPassword = _password,
                pgDbName = _dbName,
                pgConnectionString = _connectionString,
            };
            LogInformation(LogTag, "Reading config file from " + configFilePath);
            if (File.Exists(configFilePath))
            {
                LogInformation(LogTag, "Found config file");
                string dataAsJson = File.ReadAllText(configFilePath);
                PostgreSQLConfig replacingConfig = JsonConvert.DeserializeObject<PostgreSQLConfig>(dataAsJson);
                if (replacingConfig.pgAddress != null)
                    config.pgAddress = replacingConfig.pgAddress;
                if (replacingConfig.pgPort.HasValue)
                    config.pgPort = replacingConfig.pgPort.Value;
                if (replacingConfig.pgUsername != null)
                    config.pgUsername = replacingConfig.pgUsername;
                if (replacingConfig.pgPassword != null)
                    config.pgPassword = replacingConfig.pgPassword;
                if (replacingConfig.pgDbName != null)
                    config.pgDbName = replacingConfig.pgDbName;
                if (replacingConfig.pgConnectionString != null)
                    config.pgConnectionString = replacingConfig.pgConnectionString;
                configFileFound = true;
            }

            _address = config.pgAddress;
            _port = config.pgPort.Value;
            _username = config.pgUsername;
            _password = config.pgPassword;
            _dbName = config.pgDbName;
            _connectionString = config.pgConnectionString;

            // Read configs from ENV
            string envVal;
            envVal = Environment.GetEnvironmentVariable("pgAddress");
            if (!string.IsNullOrEmpty(envVal))
                _address = envVal;
            envVal = Environment.GetEnvironmentVariable("pgPort");
            if (!string.IsNullOrEmpty(envVal) && int.TryParse(envVal, out int envPort))
                _port = envPort;
            envVal = Environment.GetEnvironmentVariable("pgUsername");
            if (!string.IsNullOrEmpty(envVal))
                _username = envVal;
            envVal = Environment.GetEnvironmentVariable("pgPassword");
            if (!string.IsNullOrEmpty(envVal))
                _password = envVal;
            envVal = Environment.GetEnvironmentVariable("pgDbName");
            if (!string.IsNullOrEmpty(envVal))
                _dbName = envVal;
            envVal = Environment.GetEnvironmentVariable("pgConnectionString");
            if (!string.IsNullOrEmpty(envVal))
                _connectionString = envVal;

            string connectionString = GetConnectionString();
            LogInformation(LogTag, $"Connecting with connection string: {connectionString}");
            _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();

            if (!configFileFound)
            {
                // Write config file
                LogInformation(LogTag, "Not found config file, creating a new one");
                if (!Directory.Exists(configFolder))
                    Directory.CreateDirectory(configFolder);
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            this.InvokeInstanceDevExtMethods("Init");
        }

        public string GetConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                return _connectionString;
            string connectionString = "Host=" + _address + ";" +
                "Port=" + _port + ";" +
                "Username=" + _username + ";" +
                (string.IsNullOrEmpty(_password) ? "" : "Password=\"" + _password + "\";") +
                "Database=" + _dbName + ";";
            return connectionString;
        }

        public const string CACHE_KEY_VALIDATE_USER_LOGIN = "VALIDATE_USER_LOGIN";
        public override async UniTask<string> ValidateUserLogin(string username, string password)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_VALIDATE_USER_LOGIN,
                connection,
                "users", "id, password", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("username", username));
            string id = string.Empty;
            if (reader.Read())
            {
                id = reader.GetString(0);
                string hashedPassword = reader.GetString(1);
                if (!UserLoginManager.VerifyPassword(password, hashedPassword))
                    id = string.Empty;
            }
            return id;
        }

        public const string CACHE_KEY_VALIDATE_ACCESS_TOKEN = "VALIDATE_ACCESS_TOKEN";
        public override async UniTask<bool> ValidateAccessToken(string userId, string accessToken)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var count = await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_VALIDATE_ACCESS_TOKEN,
                connection,
                "users",
                PostgreSQLHelpers.WhereEqualTo("id", userId),
                PostgreSQLHelpers.AndWhereEqualTo("access_token", accessToken));
            return count > 0;
        }

        public const string CACHE_KEY_UPDATE_ACCESS_TOKEN = "UPDATE_ACCESS_TOKEN";
        public override async UniTask UpdateAccessToken(string userId, string accessToken)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_ACCESS_TOKEN,
                connection, null,
                "users",
                new[] {
                    new PostgreSQLHelpers.ColumnInfo("access_token", accessToken),
                },
                PostgreSQLHelpers.WhereEqualTo("id", userId));
        }

        public const string CACHE_KET_GET_USER_LEVEL = "GET_USER_LEVEL";
        public override async UniTask<byte> GetUserLevel(string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var result = await PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KET_GET_USER_LEVEL,
                connection,
                "user_accesses", "level", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("id", userId));
            return result == null ? (byte)0 : Convert.ToByte(result);
        }

        public const string CACHE_KEY_GET_USER_UNBAN_TIME = "GET_USER_UNBAN_TIME";
        public override async UniTask<long> GetUserUnbanTime(string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var result = await PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KEY_GET_USER_UNBAN_TIME,
                connection,
                "user_accesses", "unban_time", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("id", userId));
            return result == null ? 0 : (long)result;
        }

        public const string CACHE_KET_COUNT_USER_CURRENCIES = "COUNT_USER_CURRENCIES";
        public const string CACHE_KET_CREATE_USER_CURRENCIES = "CREATE_USER_CURRENCIES";
        public async UniTask PrepareUserCurrencies(string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var count = await PostgreSQLHelpers.ExecuteCount(
                CACHE_KET_COUNT_USER_CURRENCIES,
                connection,
                "user_currencies",
                PostgreSQLHelpers.WhereEqualTo("id", userId));
            if (count <= 0)
            {
                // Insert a new one with some amount of currencies
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KET_CREATE_USER_CURRENCIES,
                    connection, null,
                    "user_currencies",
                    new PostgreSQLHelpers.ColumnInfo("id", userId),
                    new PostgreSQLHelpers.ColumnInfo("gold", 0),
                    new PostgreSQLHelpers.ColumnInfo("cash", 0));
            }
        }

        public const string CACHE_KET_GET_GOLD = "GET_GOLD";
        public override async UniTask<int> GetGold(string userId)
        {
            await PrepareUserCurrencies(userId);
            using var connection = await _dataSource.OpenConnectionAsync();
            var result = await PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KET_GET_GOLD,
                connection,
                "user_currencies", "gold", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("id", userId));
            return result == null ? 0 : (int)result;
        }

        public override async UniTask<int> ChangeGold(string userId, int gold)
        {
            await PrepareUserCurrencies(userId);
            using var connection = await _dataSource.OpenConnectionAsync();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE user_currencies SET gold = gold + $1 WHERE id = $2 RETURNING gold", connection);
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = gold;
            cmd.Parameters[1].Value = userId;
            object result = await cmd.ExecuteScalarAsync();
            return result == null ? 0 : (int)result;
        }

        public const string CACHE_KET_GET_CASH = "GET_CASH";
        public override async UniTask<int> GetCash(string userId)
        {
            await PrepareUserCurrencies(userId);
            using var connection = await _dataSource.OpenConnectionAsync();
            var result = await PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KET_GET_CASH,
                connection,
                "user_currencies", "cash", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("id", userId));
            return result == null ? 0 : (int)result;
        }

        public override async UniTask<int> ChangeCash(string userId, int cash)
        {
            await PrepareUserCurrencies(userId);
            using var connection = await _dataSource.OpenConnectionAsync();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE user_currencies SET cash = cash + $1 WHERE id = $2 RETURNING cash", connection);
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = cash;
            cmd.Parameters[1].Value = userId;
            object result = await cmd.ExecuteScalarAsync();
            return result == null ? 0 : (int)result;
        }

        public const string CACHE_KEY_CREATE_USER_LOGIN_USERS = "CREATE_USER_LOGIN_USERS";
        public const string CACHE_KEY_CREATE_USER_LOGIN_ACCESSES = "CREATE_USER_LOGIN_ACCESSES";
        public const string CACHE_KEY_CREATE_USER_LOGIN_CURRENCIES = "CREATE_USER_LOGIN_CURRENCIES";
        public override async UniTask CreateUserLogin(string username, string password, string email)
        {
            var id = UserLoginManager.GenerateNewId();
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_CREATE_USER_LOGIN_USERS,
                    connection, transaction,
                    "users",
                    new PostgreSQLHelpers.ColumnInfo("id", id),
                    new PostgreSQLHelpers.ColumnInfo("username", username),
                    new PostgreSQLHelpers.ColumnInfo("password", UserLoginManager.GetHashedPassword(password)),
                    new PostgreSQLHelpers.ColumnInfo("email", string.IsNullOrWhiteSpace(email) ? null : email));

                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_CREATE_USER_LOGIN_ACCESSES,
                    connection, transaction,
                    "user_accesses",
                    new PostgreSQLHelpers.ColumnInfo("id", id));

                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_CREATE_USER_LOGIN_CURRENCIES,
                    connection, transaction,
                    "user_currencies",
                    new PostgreSQLHelpers.ColumnInfo("id", id));

                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "@CreateUserLogin_Transaction1");
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public const string CACHE_KEY_FIND_USERNAME = "FIND_USERNAME";
        public override async UniTask<long> FindUsername(string username)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var count = await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_FIND_USERNAME,
                connection,
                "users",
                PostgreSQLHelpers.WhereLike("username", username));
            return count;
        }

        public const string CACHE_KEY_SET_USER_UNBAN_TIME_BY_CHARACTER_NAME_SELECT_USER_ID = "SET_USER_UNBAN_TIME_BY_CHARACTER_NAME_SELECT_USER_ID";
        public const string CACHE_KEY_SET_USER_UNBAN_TIME_BY_CHARACTER_NAME_UPDATE = "SET_USER_UNBAN_TIME_BY_CHARACTER_NAME_UPDATE";
        public override async UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime)
        {
            var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_SET_USER_UNBAN_TIME_BY_CHARACTER_NAME_SELECT_USER_ID,
                connection,
                "characters", "user_id", "LIMIT 1",
                PostgreSQLHelpers.WhereLike("LOWER(character_name)", characterName.ToLower()));
            string userId = null;
            if (reader.Read())
            {
                userId = reader.GetString(0);
            }
            if (string.IsNullOrWhiteSpace(userId))
                return;
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_SET_USER_UNBAN_TIME_BY_CHARACTER_NAME_UPDATE,
                connection, null,
                "user_accesses", "id",
                new PostgreSQLHelpers.ColumnInfo("unban_time", unbanTime),
                new PostgreSQLHelpers.ColumnInfo("id", userId));
        }

        public const string CACHE_KEY_SET_CHARACTER_UNMUTE_TIME_BY_NAME = "SET_CHARACTER_UNMUTE_TIME_BY_NAME";
        public override async UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_SET_CHARACTER_UNMUTE_TIME_BY_NAME,
                connection, null,
                "characters",
                new[] {
                    new PostgreSQLHelpers.ColumnInfo("unmute_time", unmuteTime),
                },
                PostgreSQLHelpers.WhereLike("LOWER(character_name)", characterName.ToLower()));
        }

        public const string CACHE_KEY_VALIDATE_EMAIL_VERIFICATION = "VALIDATE_EMAIL_VERIFICATION";
        public override async UniTask<bool> ValidateEmailVerification(string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var count = await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_VALIDATE_EMAIL_VERIFICATION,
                connection,
                "users",
                PostgreSQLHelpers.WhereEqualTo("id", userId),
                PostgreSQLHelpers.AndWhereEqualTo("is_verify", true));
            return count > 0;
        }

        public const string CACHE_KEY_FIND_EMAIL = "FIND_EMAIL";
        public override async UniTask<long> FindEmail(string email)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var count = await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_FIND_EMAIL,
                connection,
                "users",
                PostgreSQLHelpers.WhereLike("LOWER(email)", email.ToLower()));
            return count;
        }

        public const string CACHE_KEY_UPDATE_USER_COUNT = "UPDATE_USER_COUNT";
        public override async UniTask UpdateUserCount(int userCount)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_UPDATE_USER_COUNT,
                connection, null,
                "server_statistic", "id",
                new PostgreSQLHelpers.ColumnInfo("user_count", userCount),
                new PostgreSQLHelpers.ColumnInfo("id", 1));
        }
    }
}
#endif
