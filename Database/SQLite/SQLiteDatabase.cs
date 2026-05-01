#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using Newtonsoft.Json;
using System;
using System.IO;
#endif

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase : BaseDatabase
    {
        public static readonly string LogTag = nameof(SQLiteDatabase);

#if UNITY_2017_1_OR_NEWER
        [SerializeField]
#endif
        private string dbPath = "./mmorpgtemplate.sqlite3";

#if UNITY_2017_1_OR_NEWER
        [Header("Running In Editor")]
        [SerializeField]
        [Tooltip("You should set this to where you build app to make database path as same as map server")]
        private string editorDbPath = "./mmorpgtemplate.sqlite3";
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
#nullable enable
        private SqliteConnection? _connection;
#nullable restore

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
        private event DbGetCharacterDelegate onGetCharacter = null;
        private event DbCreateCharacterDelegate<SqliteConnection, SqliteTransaction> onCreateCharacter = null;
        private event DbUpdateCharacterDelegate<SqliteConnection, SqliteTransaction> onUpdateCharacter = null;
        private event DbDeleteCharacterDelegate<SqliteConnection, SqliteTransaction> onDeleteCharacter = null;
#endif

        public override void Initialize()
        {
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/sqliteConfig.json";
            SQLiteConfig config = new SQLiteConfig()
            {
                sqliteDbPath = dbPath,
            };
            LogInformation(LogTag, "Reading config file from " + configFilePath);
            if (File.Exists(configFilePath))
            {
                LogInformation(LogTag, "Found config file");
                string dataAsJson = File.ReadAllText(configFilePath);
                SQLiteConfig replacingConfig = JsonConvert.DeserializeObject<SQLiteConfig>(dataAsJson);
                if (replacingConfig.sqliteDbPath != null)
                    config.sqliteDbPath = replacingConfig.sqliteDbPath;
                configFileFound = true;
            }

            dbPath = config.sqliteDbPath;

            // Read configs from ENV
            string envVal;
            envVal = Environment.GetEnvironmentVariable("sqliteDbPath");
            if (!string.IsNullOrEmpty(envVal))
                dbPath = envVal;

            if (!configFileFound)
            {
                // Write config file
                LogInformation(LogTag, "Not found config file, creating a new one");
                if (!Directory.Exists(configFolder))
                    Directory.CreateDirectory(configFolder);
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }

            _connection = NewConnection();
            _connection.Open();
            Init();
            this.InvokeInstanceDevExtMethods("Init");
        }

        public override void Destroy()
        {
            if (_connection != null)
                _connection.Close();
        }

        private void Init()
        {
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterattribute (
              id TEXT NOT NULL PRIMARY KEY,
              idx INTEGER NOT NULL,
              characterId TEXT NOT NULL,
              dataId INTEGER NOT NULL,
              amount INTEGER NOT NULL,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS charactercurrency (
              id TEXT NOT NULL PRIMARY KEY,
              idx INTEGER NOT NULL,
              characterId TEXT NOT NULL,
              dataId INTEGER NOT NULL,
              amount INTEGER NOT NULL,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterbuff (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              type INTEGER NOT NULL,
              dataId INTEGER NOT NULL,
              level INTEGER NOT NULL,
              buffRemainsDuration REAL NOT NULL,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterhotkey (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hotkeyId TEXT NOT NULL,
              type INTEGER NOT NULL,
              relateId TEXT NOT NULL DEFAULT '',
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characteritem (
              id TEXT NOT NULL PRIMARY KEY,
              idx INTEGER NOT NULL,
              inventoryType INTEGER NOT NULL,
              characterId TEXT NOT NULL,
              dataId INTERGER NOT NULL,
              level INTEGER NOT NULL,
              amount INTEGER NOT NULL,
              equipSlotIndex INTEGER NOT NULL DEFAULT 0,
              durability REAL NOT NULL DEFAULT 0,
              exp INTEGER NOT NULL DEFAULT 0,
              lockRemainsDuration REAL NOT NULL DEFAULT 0,
              expireTime INTEGER NOT NULL DEFAULT 0,
              randomSeed INTEGER NOT NULL DEFAULT 0,
              ammo INTEGER NOT NULL DEFAULT 0,
              sockets TEXT NOT NULL DEFAULT '',
              version INTEGER NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS storageitem (
              id TEXT NOT NULL PRIMARY KEY,
              idx INTEGER NOT NULL,
              storageType INTEGER NOT NULL,
              storageOwnerId TEXT NOT NULL,
              dataId INTERGER NOT NULL,
              level INTEGER NOT NULL,
              amount INTEGER NOT NULL,
              durability REAL NOT NULL DEFAULT 0,
              exp INTEGER NOT NULL DEFAULT 0,
              lockRemainsDuration REAL NOT NULL DEFAULT 0,
              expireTime INTEGER NOT NULL DEFAULT 0,
              randomSeed INTEGER NOT NULL DEFAULT 0,
              ammo INTEGER NOT NULL DEFAULT 0,
              sockets TEXT NOT NULL DEFAULT '',
              version INTEGER NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");


            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS storage_reservation (
                storageType INTEGER NOT NULL,
                storageOwnerId TEXT NOT NULL,
                reserverId TEXT NOT NULL,
                PRIMARY KEY (storageType, storageOwnerId)
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterquest (
              id TEXT NOT NULL PRIMARY KEY,
              idx INTEGER NOT NULL,
              characterId TEXT NOT NULL,
              dataId INTEGER NOT NULL,
              randomTasksIndex INTEGER NOT NULL DEFAULT 0,
              isComplete INTEGER NOT NULL DEFAULT 0,
              completeTime INTEGER NOT NULL DEFAULT 0,
              isTracking INTEGER NOT NULL DEFAULT 0,
              killedMonsters TEXT NOT NULL DEFAULT '',
              completedTasks TEXT NOT NULL DEFAULT '',
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characters (
              id TEXT NOT NULL PRIMARY KEY,
              userId TEXT NOT NULL,
              dataId INGETER NOT NULL DEFAULT 0,
              entityId INGETER NOT NULL DEFAULT 0,
              factionId INGETER NOT NULL DEFAULT 0,
              characterName TEXT NOT NULL UNIQUE,
              level INTEGER NOT NULL,
              exp INTEGER NOT NULL,
              currentHp INTEGER NOT NULL,
              currentMp INTEGER NOT NULL,
              currentStamina INTEGER NOT NULL,
              currentFood INTEGER NOT NULL,
              currentWater INTEGER NOT NULL,
              equipWeaponSet INTEGER NOT NULL DEFAULT 0,
              statPoint REAL NOT NULL DEFAULT 0,
              skillPoint REAL NOT NULL DEFAULT 0,
              gold INTEGER NOT NULL DEFAULT 0,
              partyId INTEGER NOT NULL DEFAULT 0,
              guildId INTEGER NOT NULL DEFAULT 0,
              guildRole INTEGER NOT NULL DEFAULT 0,
              sharedGuildExp INTEGER NOT NULL DEFAULT 0,
              currentChannel TEXT NOT NULL DEFAULT '',
              currentMapName TEXT NOT NULL DEFAULT '',
              currentPositionX REAL NOT NULL DEFAULT 0,
              currentPositionY REAL NOT NULL DEFAULT 0,
              currentPositionZ REAL NOT NULL DEFAULT 0,
              currentRotationX REAL NOT NULL DEFAULT 0,
              currentRotationY REAL NOT NULL DEFAULT 0,
              currentRotationZ REAL NOT NULL DEFAULT 0,
              currentSafeArea TEXT NOT NULL DEFAULT '',
              respawnMapName TEXT NOT NULL DEFAULT '',
              respawnPositionX REAL NOT NULL DEFAULT 0,
              respawnPositionY REAL NOT NULL DEFAULT 0,
              respawnPositionZ REAL NOT NULL DEFAULT 0,
              iconDataId INTEGER NOT NULL DEFAULT 0,
              frameDataId INTEGER NOT NULL DEFAULT 0,
              titleDataId INTEGER NOT NULL DEFAULT 0,
              reputation INTEGER NOT NULL DEFAULT 0,
              lastDeadTime INTEGER NOT NULL DEFAULT 0,
              unmuteTime INTEGER NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterskill (
              id TEXT NOT NULL PRIMARY KEY,
              idx INTEGER NOT NULL,
              characterId TEXT NOT NULL,
              dataId INTEGER NOT NULL,
              level INTEGER NOT NULL,
              coolDownRemainsDuration REAL NOT NULL,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterskillusage (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              type INTEGER NOT NULL DEFAULT 0,
              dataId INTEGER NOT NULL DEFAULT 0,
              coolDownRemainsDuration REAL NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS charactersummon (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              type INTEGER NOT NULL DEFAULT 0,
              sourceId TEXT NOT NULL DEFAULT '',
              dataId INTEGER NOT NULL DEFAULT 0,
              summonRemainsDuration REAL NOT NULL DEFAULT 0,
              level INTEGER NOT NULL DEFAULT 0,
              exp INTEGER NOT NULL DEFAULT 0,
              currentHp INTEGER NOT NULL DEFAULT 0,
              currentMp INTEGER NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS charactermount (
              id TEXT NOT NULL PRIMARY KEY,
              type INTEGER NOT NULL DEFAULT 0,
              sourceId TEXT NULL,
              mountRemainsDuration REAL NOT NULL DEFAULT 0,
              level INTEGER NOT NULL DEFAULT 0,
              currentHp INTEGER NOT NULL DEFAULT 0,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_server_boolean (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_server_int32 (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_server_float32 (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value REAL NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_private_boolean (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_private_int32 (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_private_float32 (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value REAL NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_public_boolean (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_public_int32 (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_public_float32 (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              hashedKey INTEGER NOT NULL,
              value REAL NOT NULL DEFAULT 0
            )");


            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_pk (
              id TEXT NOT NULL PRIMARY KEY,
              isPkOn BOOLEAN NOT NULL DEFAULT 0,
              lastPkOnTime INTEGER NOT NULL DEFAULT 0,
              pkPoint INTEGER NOT NULL DEFAULT 0,
              consecutivePkKills INTEGER NOT NULL DEFAULT 0,
              highestPkPoint INTEGER NOT NULL DEFAULT 0,
              highestConsecutivePkKills INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS summonbuffs (
              id TEXT NOT NULL PRIMARY KEY,
              characterId TEXT NOT NULL,
              buffId TEXT NOT NULL,
              type INTEGER NOT NULL,
              dataId INTEGER NOT NULL,
              level INTEGER NOT NULL,
              buffRemainsDuration REAL NOT NULL,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS friend (
              id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              characterId1 TEXT NOT NULL,
              characterId2 TEXT NOT NULL,
              state INTEGER NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS userlogin (
              id TEXT NOT NULL PRIMARY KEY,
              username TEXT NOT NULL UNIQUE,
              password TEXT NOT NULL,
              gold INTEGER NOT NULL DEFAULT 0,
              cash INTEGER NOT NULL DEFAULT 0,
              userLevel INTEGER NOT NULL DEFAULT 0,
              unbanTime INTEGER NOT NULL DEFAULT 0,
              email TEXT NOT NULL,
              isEmailVerified INTEGER NOT NULL DEFAULT 0,
              authType INTEGER NOT NULL,
              accessToken TEXT,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS buildings (
              id TEXT NOT NULL PRIMARY KEY,
              channel TEXT NOT NULL DEFAULT 'default',
              parentId TEXT NOT NULL,
              entityId INGETER NOT NULL,
              currentHp INTEGER NOT NULL DEFAULT 0,
              remainsLifeTime REAL NOT NULL DEFAULT 0,
              mapName TEXT NOT NULL,
              positionX REAL NOT NULL,
              positionY REAL NOT NULL,
              positionZ REAL NOT NULL,
              rotationX REAL NOT NULL,
              rotationY REAL NOT NULL,
              rotationZ REAL NOT NULL,
              isLocked INTEGER NOT NULL DEFAULT 0,
              lockPassword TEXT NOT NULL DEFAULT '',
              creatorId TEXT NOT NULL,
              creatorName TEXT NOT NULL,
              extraData TEXT NOT NULL DEFAULT '',
              isSceneObject INTEGER NOT NULL DEFAULT 0,
              createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS guild (
              id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              guildName TEXT NOT NULL,
              leaderId TEXT NOT NULL,
              level INTEGER NOT NULL DEFAULT 1,
              exp INTEGER NOT NULL DEFAULT 0,
              skillPoint INTEGER NOT NULL DEFAULT 0,
              guildMessage TEXT NOT NULL DEFAULT '',
              guildMessage2 TEXT NOT NULL DEFAULT '',
              gold INTEGER NOT NULL DEFAULT 0,
              score INTEGER NOT NULL DEFAULT 0,
              options TEXT NOT NULL DEFAULT '',
              autoAcceptRequests INTEGER NOT NULL DEFAULT 0,
              rank INTEGER NOT NULL DEFAULT 0,
              currentMembers INTEGER NOT NULL DEFAULT 0,
              maxMembers INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS guildrole (
              guildId INTEGER NOT NULL,
              guildRole INTEGER NOT NULL,
              name TEXT NOT NULL,
              canInvite INTEGER NOT NULL DEFAULT 0,
              canKick INTEGER NOT NULL DEFAULT 0,
              canUseStorage INTEGER NOT NULL DEFAULT 0,
              shareExpPercentage INTEGER NOT NULL DEFAULT 0
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS guildskill (
              guildId INTEGER NOT NULL,
              dataId INTEGER NOT NULL,
              level INTEGER NOT NULL
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS guildrequest (
              id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              guildId INTEGER NOT NULL,
              requesterId TEXT NOT NULL
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS party (
              id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              shareExp INTEGER NOT NULL,
              shareItem INTEGER NOT NULL,
              leaderId TEXT NOT NULL
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS mail (
              id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              eventId TEXT NULL DEFAULT NULL,
              senderId TEXT NULL DEFAULT NULL,
              senderName TEXT NULL DEFAULT NULL,
              receiverId TEXT NOT NULL,
              title TEXT NOT NULL,
              content TEXT NOT NULL,
              gold INTEGER NOT NULL DEFAULT 0,
              cash INTEGER NOT NULL DEFAULT 0,
              currencies TEXT NOT NULL,
              items TEXT NOT NULL,
              isRead INTEGER NOT NULL DEFAULT 0,
              readTimestamp TIMESTAMP NULL DEFAULT NULL,
              isClaim INTEGER NOT NULL DEFAULT 0,
              claimTimestamp TIMESTAMP NULL DEFAULT NULL,
              isDelete INTEGER NOT NULL DEFAULT 0,
              deleteTimestamp TIMESTAMP NULL DEFAULT NULL,
              sentTimestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            )");

            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS statistic (
              userCount INTEGER NOT NULL DEFAULT 0
            )");
        }

        private bool IsColumnExist(string tableName, string findingColumn)
        {
            using (SqliteCommand cmd = new SqliteCommand("PRAGMA table_info(" + tableName + ");", _connection))
            {
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(1).Equals(findingColumn))
                        return true;
                }
                reader.Close();
            }
            return false;
        }

        private bool IsColumnType(string tableName, string findingColumn, string type)
        {
            using (SqliteCommand cmd = new SqliteCommand("PRAGMA table_info(" + tableName + ");", _connection))
            {
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(1).Equals(findingColumn))
                        return reader.GetString(2).ToLower().Equals(type.ToLower());
                }
                reader.Close();
            }
            return false;
        }

        public string GetConnectionString()
        {
            string path = dbPath;

#if UNITY_2017_1_OR_NEWER
            if (Application.isMobilePlatform)
            {
                if (path.StartsWith("./"))
                    path = path.Substring(1);
                if (!path.StartsWith("/"))
                    path = "/" + path;
                path = Application.persistentDataPath + path;
            }

            if (Application.isEditor)
                path = editorDbPath;

            if (!File.Exists(path))
                SqliteConnection.CreateFile(path);
#endif

            return "Data Source=" + path;
        }

        public SqliteConnection NewConnection()
        {
            return new SqliteConnection(GetConnectionString());
        }

        public int ExecuteNonQuery(string sql, params SqliteParameter[] args)
        {
            return ExecuteNonQuery(null, sql, args);
        }

        public int ExecuteNonQuery(SqliteTransaction transaction, string sql, params SqliteParameter[] args)
        {
            int numRows = 0;
            using (SqliteCommand cmd = new SqliteCommand(sql, _connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (SqliteParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    numRows = cmd.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            return numRows;
        }

        public object ExecuteScalar(string sql, params SqliteParameter[] args)
        {
            return ExecuteScalar(null, sql, args);
        }

        public object ExecuteScalar(SqliteTransaction transaction, string sql, params SqliteParameter[] args)
        {
            object result = null;
            using (SqliteCommand cmd = new SqliteCommand(sql, _connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (SqliteParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    result = cmd.ExecuteScalar();
                }
                catch (SqliteException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            return result;
        }

        public void ExecuteReader(Action<SqliteDataReader> onRead, string sql, params SqliteParameter[] args)
        {
            ExecuteReader(null, onRead, sql, args);
        }

        public void ExecuteReader(SqliteTransaction transaction, Action<SqliteDataReader> onRead, string sql, params SqliteParameter[] args)
        {
            using (SqliteCommand cmd = new SqliteCommand(sql, _connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (SqliteParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    SqliteDataReader dataReader = cmd.ExecuteReader();
                    if (onRead != null) onRead.Invoke(dataReader);
                    dataReader.Close();
                }
                catch (SqliteException ex)
                {
                    LogException(LogTag, ex);
                }
            }
        }

        public override UniTask<string> ValidateUserLogin(string username, string password)
        {
            string id = string.Empty;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    id = reader.GetString(0);
                    string hashedPassword = reader.GetString(1);
                    if (!UserLoginManager.VerifyPassword(password, hashedPassword))
                        id = string.Empty;
                }
            }, "SELECT id, password FROM userlogin WHERE username=@username AND authType=@authType LIMIT 1",
                new SqliteParameter("@username", username),
                new SqliteParameter("@authType", AUTH_TYPE_NORMAL));

            return new UniTask<string>(id);
        }

        public override UniTask<bool> ValidateAccessToken(string userId, string accessToken)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE id=@id AND accessToken=@accessToken",
                new SqliteParameter("@id", userId),
                new SqliteParameter("@accessToken", accessToken));
            return new UniTask<bool>((result != null ? (long)result : 0) > 0);
        }

        public override UniTask<byte> GetUserLevel(string userId)
        {
            byte userLevel = 0;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                    userLevel = (byte)reader.GetInt32(0);
            }, "SELECT userLevel FROM userlogin WHERE id=@id LIMIT 1",
                new SqliteParameter("@id", userId));
            return new UniTask<byte>(userLevel);
        }

        public override UniTask<int> GetGold(string userId)
        {
            int gold = 0;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM userlogin WHERE id=@id LIMIT 1",
                new SqliteParameter("@id", userId));
            return new UniTask<int>(gold);
        }

        public override UniTask<int> ChangeGold(string userId, int gold)
        {
            ExecuteNonQuery("UPDATE userlogin SET gold = gold + @gold WHERE id=@id",
                new SqliteParameter("@id", userId),
                new SqliteParameter("@gold", gold));
            return GetGold(userId);
        }

        public override UniTask<int> GetCash(string userId)
        {
            int cash = 0;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                    cash = reader.GetInt32(0);
            }, "SELECT cash FROM userlogin WHERE id=@id LIMIT 1",
                new SqliteParameter("@id", userId));
            return new UniTask<int>(cash);
        }

        public override UniTask<int> ChangeCash(string userId, int cash)
        {
            ExecuteNonQuery("UPDATE userlogin SET cash = cash + @cash WHERE id=@id",
                new SqliteParameter("@id", userId),
                new SqliteParameter("@cash", cash));
            return GetCash(userId);
        }

        public override UniTask UpdateAccessToken(string userId, string accessToken)
        {
            ExecuteNonQuery("UPDATE userlogin SET accessToken=@accessToken WHERE id=@id",
                new SqliteParameter("@id", userId),
                new SqliteParameter("@accessToken", accessToken));
            return new UniTask();
        }

        public override UniTask CreateUserLogin(string username, string password, string email)
        {
            ExecuteNonQuery("INSERT INTO userlogin (id, username, password, email, authType) VALUES (@id, @username, @password, @email, @authType)",
                new SqliteParameter("@id", UserLoginManager.GenerateNewId()),
                new SqliteParameter("@username", username),
                new SqliteParameter("@password", UserLoginManager.GetHashedPassword(password)),
                new SqliteParameter("@email", email),
                new SqliteParameter("@authType", AUTH_TYPE_NORMAL));
            return new UniTask();
        }

        public override UniTask<long> FindUsername(string username)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE username LIKE @username",
                new SqliteParameter("@username", username));
            return new UniTask<long>(result != null ? (long)result : 0);
        }

        public override UniTask<long> GetUserUnbanTime(string userId)
        {
            long unbanTime = 0;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    unbanTime = reader.GetInt64(0);
                }
            }, "SELECT unbanTime FROM userlogin WHERE id=@id LIMIT 1",
                new SqliteParameter("@id", userId));
            return new UniTask<long>(unbanTime);
        }

        public override UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime)
        {
            string userId = string.Empty;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    userId = reader.GetString(0);
                }
            }, "SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new SqliteParameter("@characterName", characterName));
            if (string.IsNullOrEmpty(userId))
                return new UniTask();
            ExecuteNonQuery("UPDATE userlogin SET unbanTime=@unbanTime WHERE id=@id",
                new SqliteParameter("@id", userId),
                new SqliteParameter("@unbanTime", unbanTime));
            return new UniTask();
        }

        public override UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime)
        {
            ExecuteNonQuery("UPDATE characters SET unmuteTime=@unmuteTime WHERE characterName LIKE @characterName",
                new SqliteParameter("@characterName", characterName),
                new SqliteParameter("@unmuteTime", unmuteTime));
            return new UniTask();
        }

        public override UniTask<bool> ValidateEmailVerification(string userId)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE userId=@userId AND isEmailVerified=1",
                new SqliteParameter("@userId", userId));
            return new UniTask<bool>((result != null ? (long)result : 0) > 0);
        }

        public override UniTask<long> FindEmail(string email)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE email LIKE @email",
                new SqliteParameter("@email", email));
            return new UniTask<long>(result != null ? (long)result : 0);
        }

        public override UniTask UpdateUserCount(int userCount)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM statistic WHERE 1");
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                ExecuteNonQuery("UPDATE statistic SET userCount=@userCount;",
                    new SqliteParameter("@userCount", userCount));
            }
            else
            {
                ExecuteNonQuery("INSERT INTO statistic (userCount) VALUES(@userCount);",
                    new SqliteParameter("@userCount", userCount));
            }
            return new UniTask();
        }
#endif
        }
    }
