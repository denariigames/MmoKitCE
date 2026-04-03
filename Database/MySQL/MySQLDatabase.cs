#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.IO;
#endif

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase : BaseDatabase
    {
        public static readonly string LogTag = nameof(MySQLDatabase);

#if UNITY_2017_1_OR_NEWER
        [SerializeField]
#endif
        private string address = "127.0.0.1";
#if UNITY_2017_1_OR_NEWER
        [SerializeField]
#endif
        private int port = 3306;
#if UNITY_2017_1_OR_NEWER
        [SerializeField]
#endif
        private string username = "root";
#if UNITY_2017_1_OR_NEWER
        [SerializeField]
#endif
        private string password = "";
#if UNITY_2017_1_OR_NEWER
        [SerializeField]
#endif
        private string dbName = "mmorpgtemplate";
#if UNITY_2017_1_OR_NEWER
        [SerializeField]
        [Tooltip("Leave this empty to use `address`, `port`, `username`, `password` and `dbName` to build connection string")]
#endif
        private string connectionString = "";

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
        private event DbGetCharacterDelegate onGetCharacter = null;
        private event DbCreateCharacterDelegate<MySqlConnection, MySqlTransaction> onCreateCharacter = null;
        private event DbUpdateCharacterDelegate<MySqlConnection, MySqlTransaction> onUpdateCharacter = null;
        private event DbDeleteCharacterDelegate<MySqlConnection, MySqlTransaction> onDeleteCharacter = null;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
        public override void Initialize()
        {
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/mySqlConfig.json";
            MySQLConfig config = new MySQLConfig()
            {
                mySqlAddress = address,
                mySqlPort = port,
                mySqlUsername = username,
                mySqlPassword = password,
                mySqlDbName = dbName,
                mySqlConnectionString = connectionString,
            };
            LogInformation(LogTag, "Reading config file from " + configFilePath);
            if (File.Exists(configFilePath))
            {
                LogInformation(LogTag, "Found config file");
                string dataAsJson = File.ReadAllText(configFilePath);
                MySQLConfig replacingConfig = JsonConvert.DeserializeObject<MySQLConfig>(dataAsJson);
                if (replacingConfig.mySqlAddress != null)
                    config.mySqlAddress = replacingConfig.mySqlAddress;
                if (replacingConfig.mySqlPort.HasValue)
                    config.mySqlPort = replacingConfig.mySqlPort.Value;
                if (replacingConfig.mySqlUsername != null)
                    config.mySqlUsername = replacingConfig.mySqlUsername;
                if (replacingConfig.mySqlPassword != null)
                    config.mySqlPassword = replacingConfig.mySqlPassword;
                if (replacingConfig.mySqlDbName != null)
                    config.mySqlDbName = replacingConfig.mySqlDbName;
                if (replacingConfig.mySqlConnectionString != null)
                    config.mySqlConnectionString = replacingConfig.mySqlConnectionString;
                configFileFound = true;
            }

            address = config.mySqlAddress;
            port = config.mySqlPort.Value;
            username = config.mySqlUsername;
            password = config.mySqlPassword;
            dbName = config.mySqlDbName;
            connectionString = config.mySqlConnectionString;

            // Read configs from ENV
            string envVal;
            envVal = Environment.GetEnvironmentVariable("mySqlAddress");
            if (!string.IsNullOrEmpty(envVal))
                address = envVal;
            envVal = Environment.GetEnvironmentVariable("mySqlPort");
            if (!string.IsNullOrEmpty(envVal) && int.TryParse(envVal, out int envPort))
                port = envPort;
            envVal = Environment.GetEnvironmentVariable("mySqlUsername");
            if (!string.IsNullOrEmpty(envVal))
                username = envVal;
            envVal = Environment.GetEnvironmentVariable("mySqlPassword");
            if (!string.IsNullOrEmpty(envVal))
                password = envVal;
            envVal = Environment.GetEnvironmentVariable("mySqlDbName");
            if (!string.IsNullOrEmpty(envVal))
                dbName = envVal;
            envVal = Environment.GetEnvironmentVariable("mySqlConnectionString");
            if (!string.IsNullOrEmpty(envVal))
                connectionString = envVal;

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
            string connectionString;
            if (!string.IsNullOrWhiteSpace(this.connectionString))
            {
                connectionString = this.connectionString;
                if (!connectionString.Contains("Convert Zero Datetime"))
                    connectionString += $";Convert Zero Datetime=true";
                return connectionString;
            }
            connectionString = "Server=" + address + ";" +
            "Port=" + port + ";" +
            "Uid=" + username + ";" +
                (string.IsNullOrEmpty(password) ? "" : "Pwd=\"" + password + "\";") +
                "Database=" + dbName + ";" +
                "SSL Mode=None;";
            if (!connectionString.Contains("Convert Zero Datetime"))
                connectionString += $";Convert Zero Datetime=true";
            return connectionString;
        }

        public MySqlConnection NewConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }

        private async UniTask OpenConnection(MySqlConnection connection)
        {
            try
            {
                await connection.OpenAsync();
            }
            catch (MySqlException ex)
            {
                LogException(LogTag, ex);
            }
        }

        public async UniTask<long> ExecuteInsertData(string sql, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                long result = await ExecuteInsertData(connection, null, sql, args);
                return result;
            }
        }

        public async UniTask<long> ExecuteInsertData(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            return await ExecuteInsertData(connection, transaction, false, sql, args);
        }

        public async UniTask<long> ExecuteInsertData(MySqlConnection connection, MySqlTransaction transaction, bool isAsync, string sql, params MySqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            long result = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    if (isAsync)
                    {
                        await cmd.PrepareAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    result = cmd.LastInsertedId;
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
            return result;
        }

        public async UniTask<int> ExecuteNonQuery(string sql, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                int result = await ExecuteNonQuery(connection, null, sql, args);
                return result;
            }
        }

        public async UniTask<int> ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            return await ExecuteNonQuery(connection, transaction, false, sql, args);
        }

        public async UniTask<int> ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, bool isAsync, string sql, params MySqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            int numRows = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    if (isAsync)
                    {
                        await cmd.PrepareAsync();
                        numRows = await cmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        cmd.Prepare();
                        numRows = cmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
            return numRows;
        }

        public async UniTask<object> ExecuteScalar(string sql, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                object result = await ExecuteScalar(connection, null, sql, args);
                return result;
            }
        }

        public async UniTask<object> ExecuteScalar(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            return await ExecuteScalar(connection, transaction, false, sql, args);
        }

        public async UniTask<object> ExecuteScalar(MySqlConnection connection, MySqlTransaction transaction, bool isAsync, string sql, params MySqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            object result = null;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    if (isAsync)
                    {
                        await cmd.PrepareAsync();
                        result = await cmd.ExecuteScalarAsync();
                    }
                    else
                    {
                        cmd.Prepare();
                        result = cmd.ExecuteScalar();
                    }
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
            return result;
        }

        public async UniTask ExecuteReader(Action<MySqlDataReader> onRead, string sql, params MySqlParameter[] args)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                await ExecuteReader(connection, null, onRead, sql, args);
            }
        }

        public async UniTask ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, Action<MySqlDataReader> onRead, string sql, params MySqlParameter[] args)
        {
            await ExecuteReader(connection, transaction, onRead, false, sql, args);
        }

        public async UniTask ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, Action<MySqlDataReader> onRead, bool isAsync, string sql, params MySqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    if (isAsync)
                    {
                        await cmd.PrepareAsync();
                        using (MySqlDataReader dataReader = await cmd.ExecuteReaderAsync())
                        {
                            if (onRead != null)
                                onRead.Invoke(dataReader);
                        }
                    }
                    else
                    {
                        cmd.Prepare();
                        using (MySqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (onRead != null)
                                onRead.Invoke(dataReader);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
        }

        public override async UniTask<string> ValidateUserLogin(string username, string password)
        {
            string id = string.Empty;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    id = reader.GetString(0);
                    string hashedPassword = reader.GetString(1);
                    if (!UserLoginManager.VerifyPassword(password, hashedPassword))
                        id = string.Empty;
                }
            }, "SELECT id, password FROM userlogin WHERE username=@username AND authType=@authType LIMIT 1",
                new MySqlParameter("@username", username),
                new MySqlParameter("@authType", AUTH_TYPE_NORMAL));

            return id;
        }

        public override async UniTask<bool> ValidateAccessToken(string userId, string accessToken)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE id=@id AND accessToken=@accessToken",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@accessToken", accessToken));
            return (result != null ? (long)result : 0) > 0;
        }

        public override async UniTask<byte> GetUserLevel(string userId)
        {
            byte userLevel = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    userLevel = reader.GetByte(0);
            }, "SELECT userLevel FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return userLevel;
        }

        public override async UniTask<int> GetGold(string userId)
        {
            int gold = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return gold;
        }

        public override async UniTask<int> ChangeGold(string userId, int gold)
        {
            await ExecuteNonQuery("UPDATE userlogin SET gold = gold + @gold WHERE id=@id",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@gold", gold));
            return await GetGold(userId);
        }

        public override async UniTask<int> GetCash(string userId)
        {
            int cash = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    cash = reader.GetInt32(0);
            }, "SELECT cash FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return cash;
        }

        public override async UniTask<int> ChangeCash(string userId, int cash)
        {
            await ExecuteNonQuery("UPDATE userlogin SET cash = cash + @cash WHERE id=@id",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@cash", cash));
            return await GetCash(userId);
        }

        public override async UniTask UpdateAccessToken(string userId, string accessToken)
        {
            await ExecuteNonQuery("UPDATE userlogin SET accessToken=@accessToken WHERE id=@id",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@accessToken", accessToken));
        }

        public override async UniTask CreateUserLogin(string username, string password, string email)
        {
            await ExecuteNonQuery("INSERT INTO userlogin (id, username, password, email, authType) VALUES (@id, @username, @password, @email, @authType)",
                new MySqlParameter("@id", UserLoginManager.GenerateNewId()),
                new MySqlParameter("@username", username),
                new MySqlParameter("@password", UserLoginManager.GetHashedPassword(password)),
                new MySqlParameter("@email", email),
                new MySqlParameter("@authType", AUTH_TYPE_NORMAL));
        }

        public override async UniTask<long> FindUsername(string username)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE username LIKE @username",
                new MySqlParameter("@username", username));
            return result != null ? (long)result : 0;
        }

        public override async UniTask<long> GetUserUnbanTime(string userId)
        {
            long unbanTime = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    unbanTime = reader.GetInt64(0);
                }
            }, "SELECT unbanTime FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return unbanTime;
        }

        public override async UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime)
        {
            string userId = string.Empty;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    userId = reader.GetString(0);
                }
            }, "SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new MySqlParameter("@characterName", characterName));
            if (string.IsNullOrEmpty(userId))
                return;
            await ExecuteNonQuery("UPDATE userlogin SET unbanTime=@unbanTime WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@unbanTime", unbanTime));
        }

        public override async UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime)
        {
            await ExecuteNonQuery("UPDATE characters SET unmuteTime=@unmuteTime WHERE characterName LIKE @characterName LIMIT 1",
                new MySqlParameter("@characterName", characterName),
                new MySqlParameter("@unmuteTime", unmuteTime));
        }

        public override async UniTask<bool> ValidateEmailVerification(string userId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE id=@userId AND isEmailVerified=1",
                new MySqlParameter("@userId", userId));
            return (result != null ? (long)result : 0) > 0;
        }

        public override async UniTask<long> FindEmail(string email)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE email LIKE @email",
                new MySqlParameter("@email", email));
            return result != null ? (long)result : 0;
        }

        public override async UniTask UpdateUserCount(int userCount)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM statistic WHERE 1");
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                await ExecuteNonQuery("UPDATE statistic SET userCount=@userCount;",
                    new MySqlParameter("@userCount", userCount));
            }
            else
            {
                await ExecuteNonQuery("INSERT INTO statistic (userCount) VALUES(@userCount);",
                    new MySqlParameter("@userCount", userCount));
            }
        }
#endif
    }
}
