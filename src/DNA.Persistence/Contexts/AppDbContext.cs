
using System;
using DNA.Domain;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DNA.Domain.Exceptions;
using DNA.Domain.Utils;

namespace DNA.Persistence.Contexts {

    public class AppDbContext : IAppDbContext {

        readonly IConfiguration _configuration;
        readonly string _defaultConnectionString;
        SqlConnection _connection;
        static bool Initialized = false;
        SqlConnection IAppDbContext.Connection {
            get {
                _connection = new SqlConnection(_defaultConnectionString);
                return _connection;
            }
        }

        SqlConnection IAppDbContext.NetsisConnection
        {
            get {
                var conStr = _configuration.GetConnectionString("Netsis");
                if (string.IsNullOrWhiteSpace(conStr))
                    throw new ArgumentNullException("ConnectionString:Netsis");
                _connection = new SqlConnection(conStr);
                return _connection;
            }
        }

        public AppDbContext(IConfiguration configuration) {
            _configuration = configuration;

            _defaultConnectionString = configuration.GetConnectionString("Default");

            lock (_lock) {
                if (!Initialized) {
                    Initialized = true;
                    lock (_lock) {
                        Init();
                    }
                }
            }
        }

        public void Dispose() {
            if (_connection != null)
                _connection.Dispose();
            _connection = null;
        }

        private readonly object _lock = new object();

        void Init() {
            using var connection = new SqlConnection(_defaultConnectionString);
            try {

                connection.Execute(SetTablePrefix(TableScripts.Log));
                connection.Execute(SetTablePrefix(TableScripts.Log_AddColumn_EntityName));
                connection.Execute(SetTablePrefix(TableScripts.Log_AddColumn_EntityKey));
                connection.Execute(SetTablePrefix(TableScripts.User));
                connection.Execute(SetTablePrefix(TableScripts.User_AddColumn_IsInitialPassword));
                connection.Execute(SetTablePrefix(TableScripts.User_Insert_Admin));
                connection.Execute(SetTablePrefix(TableScripts.User_Update_Admin));

                if (_configuration.GetSection("Config:Notification").GetValue<bool>("Enabled")) {
                    connection.Execute(SetTablePrefix(TableScripts.Notification));
                    connection.Execute(SetTablePrefix(TableScripts.Notification_AddColumn_Comment));
                    connection.Execute(SetTablePrefix(TableScripts.Notification_AddColumn_NotificationType));
                    connection.Execute(SetTablePrefix(TableScripts.Notification_AddColumn_EntityKey));
                }

                // JOB çalışırken servis durursa, JOB kaydı kapatılamaz, servis açıldığınde Hangfire JOB'ı baştan başlatacak.
                // Bu yüzden servis durduğunda yarım kalmış JOB varsa sil.
                connection.Execute(@"
                    DELETE j FROM HangFire.Job AS j
                        LEFT JOIN HangFire.JobQueue AS jq ON jq.JobId=j.Id
                        WHERE jq.[Queue]='default' AND (StateName='Enqueued' OR StateName='Processing');

                    DELETE FROM HangFire.JobQueue WHERE [Queue]='default'
                    ");

            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public string SetTablePrefix(string query) {
            return SqlTableScriptGenerator.SetTablePrefix(query, _configuration);
        }
    }
}