using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Repositories;
using DNA.Domain;
using DNA.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Microsoft.Extensions.Configuration;
using DNA.Domain.Extentions;

namespace DNA.Persistence.Repositories {

    [Service(typeof(ILogRepository), Lifetime.Scoped)]
    public class LogRepository : BaseRepository, ILogRepository {
        private readonly IConfiguration _configuration;

        public LogRepository(IAppDbContext context, IConfiguration configuration) : base(context) {
            this._configuration = configuration;
        }

        public async Task<QueryResult<Log>> ListAsync(Query query) {

            using var connection = Context.Connection;

            var sql = @" SELECT [Id],[MachineName],[Logged],[EntityName],[EntityKey],[Level],[Message],[Logger],[Callsite] FROM [{TablePrefix}NLOG] ";
            query.Take = query.Take <= 0 ? 100 : query.Take;
            query.RequireTotalCount = true;

            sql = query.GetSqlQuery(_configuration["Config:Database:LogSelectQuery"] ?? sql);

            sql = Context.SetTablePrefix(sql);
            var results = await connection.QueryMultipleAsync(sql, (object)query.SqlParameters);
            var logs = results.Read<Log>().ToList();
            var totalItems = results.ReadSingle<int>();
            return new QueryResult<Log> {
                Items = logs,
                TotalItems = totalItems,
            };
        }

        public async Task<Log> GetByIdAsync(int id) {
            using var connection = Context.Connection;
            var sql = "SELECT * FROM [{TablePrefix}NLOG] WHERE Id = @Id";
            var result = await connection.QuerySingleAsync<Log>(Context.SetTablePrefix(sql), new { Id = id });
            return result;
        }

    }
}
