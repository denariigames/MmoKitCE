#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_MAIL_LIST = "MAIL_LIST";
        public override async UniTask<List<MailListEntry>> MailList(string userId, bool onlyNewMails)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_MAIL_LIST,
                connection,
                "mails", "id, sender_name, title, gold, cash, currencies, items, is_read, is_claim, sent_time", "ORDER BY is_read ASC, sent_time DESC",
                PostgreSQLHelpers.WhereEqualTo("receiver_id", userId),
                PostgreSQLHelpers.AndWhereEqualTo("is_delete", false));
            List<MailListEntry> result = new List<MailListEntry>();
            MailListEntry tempMail;
            while (reader.Read())
            {
                int gold = reader.GetInt32(3);
                int cash = reader.GetInt32(4);
                string currencies = reader.GetString(5);
                string items = reader.GetString(6);
                tempMail = new MailListEntry()
                {
                    Id = reader.GetInt64(0).ToString(),
                    SenderName = reader.GetString(1),
                    Title = reader.GetString(2),
                    IsRead = reader.GetBoolean(7),
                    IsClaim = reader.GetBoolean(8),
                    SentTimestamp = ((DateTimeOffset)DateTime.SpecifyKind(reader.GetDateTime(9), DateTimeKind.Utc)).ToUnixTimeSeconds(),
                };
                if (onlyNewMails)
                {
                    if (!tempMail.IsClaim && (gold != 0 || cash != 0 || !string.IsNullOrEmpty(currencies) || !string.IsNullOrEmpty(items)))
                        result.Add(tempMail);
                    else if (!tempMail.IsRead)
                        result.Add(tempMail);
                }
                else
                {
                    result.Add(tempMail);
                }
            }
            return result;
        }

        public const string CACHE_KEY_GET_MAIL = "GET_MAIL";
        public override async UniTask<Mail> GetMail(string mailId, string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_MAIL,
                connection,
                "mails", "id, event_id, sender_id, sender_name, receiver_id, title, content, gold, cash, currencies, items, is_read, read_time, is_claim, claim_time, sent_time", "LIMIT 1",
                PostgreSQLHelpers.WhereBigEqualTo("id", long.Parse(mailId)),
                PostgreSQLHelpers.AndWhereEqualTo("receiver_id", userId),
                PostgreSQLHelpers.AndWhereEqualTo("is_delete", false));
            Mail result = new Mail();
            if (reader.Read())
            {
                result.Id = reader.GetInt64(0).ToString();
                result.EventId = reader.GetString(1);
                result.SenderId = reader.GetString(2);
                result.SenderName = reader.GetString(3);
                result.ReceiverId = reader.GetString(4);
                result.Title = reader.GetString(5);
                result.Content = reader.GetString(6);
                result.Gold = reader.GetInt32(7);
                result.Cash = reader.GetInt32(8);
                result.ReadCurrencies(reader.GetString(9));
                result.ReadItems(reader.GetString(10));
                result.IsRead = reader.GetBoolean(11);
                if (reader[12] != DBNull.Value)
                    result.ReadTimestamp = ((DateTimeOffset)DateTime.SpecifyKind(reader.GetDateTime(12), DateTimeKind.Utc)).ToUnixTimeSeconds();
                result.IsClaim = reader.GetBoolean(13);
                if (reader[14] != DBNull.Value)
                    result.ClaimTimestamp = ((DateTimeOffset)DateTime.SpecifyKind(reader.GetDateTime(14), DateTimeKind.Utc)).ToUnixTimeSeconds();
                result.SentTimestamp = ((DateTimeOffset)DateTime.SpecifyKind(reader.GetDateTime(15), DateTimeKind.Utc)).ToUnixTimeSeconds();
            }
            return result;
        }

        public string CACHE_KEY_UPDATE_READ_MAIL_STATE = "UPDATE_READ_MAIL_STATE";
        public override async UniTask<long> UpdateReadMailState(string mailId, string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            long count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_READ_MAIL_STATE,
                connection, null,
                "mails",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("is_read", true),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.TimestampTz, "read_time", "timezone('utc', now())"),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereBigEqualTo("id", long.Parse(mailId)),
                    PostgreSQLHelpers.AndWhereEqualTo("receiver_id", userId),
                    PostgreSQLHelpers.AndWhereEqualTo("is_read", false),
                });
            return count;
        }

        public string CACHE_KEY_UPDATE_CLAIM_MAIL_ITEMS_STATE = "UPDATE_CLAIM_MAIL_ITEMS_STATE";
        public override async UniTask<long> UpdateClaimMailItemsState(string mailId, string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            long count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_CLAIM_MAIL_ITEMS_STATE,
                connection, null,
                "mails",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("is_claim", true),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.TimestampTz, "claim_time", "timezone('utc', now())"),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereBigEqualTo("id", long.Parse(mailId)),
                    PostgreSQLHelpers.AndWhereEqualTo("receiver_id", userId),
                    PostgreSQLHelpers.AndWhereEqualTo("is_claim", false),
                });
            return count;
        }

        public string CACHE_KEY_UPDATE_DELETE_ITEMS_STATE = "UPDATE_DELETE_ITEMS_STATE";
        public override async UniTask<long> UpdateDeleteMailState(string mailId, string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            long count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_DELETE_ITEMS_STATE,
                connection, null,
                "mails",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("is_delete", true),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.TimestampTz, "delete_time", "timezone('utc', now())"),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereBigEqualTo("id", long.Parse(mailId)),
                    PostgreSQLHelpers.AndWhereEqualTo("receiver_id", userId),
                    PostgreSQLHelpers.AndWhereEqualTo("is_delete", false),
                });
            return count;
        }

        public string CACHE_KEY_CREATE_MAIL = "CREATE_MAIL";
        public override async UniTask<int> CreateMail(Mail mail)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            int id = (int)await PostgreSQLHelpers.ExecuteInsertScalar(
                CACHE_KEY_CREATE_MAIL,
                connection,
                "mails",
                new[] {
                    new PostgreSQLHelpers.ColumnInfo("event_id", mail.EventId),
                    new PostgreSQLHelpers.ColumnInfo("sender_id", mail.SenderId),
                    new PostgreSQLHelpers.ColumnInfo("sender_name", mail.SenderName),
                    new PostgreSQLHelpers.ColumnInfo("receiver_id", mail.ReceiverId),
                    new PostgreSQLHelpers.ColumnInfo("title", mail.Title),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Text, "content", mail.Content),
                    new PostgreSQLHelpers.ColumnInfo("gold", mail.Gold),
                    new PostgreSQLHelpers.ColumnInfo("cash", mail.Cash),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Text, "currencies", mail.WriteCurrencies()),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Text, "items", mail.WriteItems()),
                }, "id");
            return id;
        }

        public override async UniTask<int> GetMailNotification(string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT COUNT(*) FROM mails WHERE (((gold != 0 || currencies != '' || items != '') && !is_claim) || !is_read) AND !is_delete AND receiver_id = $1", connection);
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;
            object result = await cmd.ExecuteScalarAsync();
            return result == null ? 0 : (int)result;
        }
    }
}
#endif