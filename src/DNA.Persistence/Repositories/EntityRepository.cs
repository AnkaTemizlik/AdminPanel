using Dapper;
using DNA.Domain;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Repositories;
using DNA.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Persistence.Repositories {

    [Service(typeof(IEntityRepository), Lifetime.Scoped)]
    public class EntityRepository : BaseRepository, IEntityRepository {
        public EntityRepository(IAppDbContext context) : base(context) { }

        public async Task<T> GetByIdAsync<T>(string selectQuery, object p) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix(selectQuery);
            var result = await connection.QueryAsync<T>(sql, p);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<dynamic>> QueryAsync(string sqlQuery) {
            return await QueryAsync<dynamic>(sqlQuery);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix(sqlQuery);
            var result = await connection.QueryAsync<T>(sql);
            return result;
        }

        public async Task<QueryResult<dynamic>> QueryAsync(EntityQuery query, string sqlQuery, string orderBy = null) {
            return await QueryAsync<dynamic>(query, sqlQuery, null, orderBy);
        }

        public async Task<QueryResult<object>> QueryAsync(Type type, EntityQuery query, string sqlQuery, string sqlCountQuery, string orderBy = null) {
            using var connection = Context.Connection;

            var sql = Context.SetTablePrefix( query.GetSqlQuery(sqlQuery, orderBy));

            if (query.RequireTotalCount) {
                var results = await connection.QueryMultipleAsync(sql, (object)query.SqlParameters);
                var logs = results.Read(type).ToList();
                var totalItems = results.ReadSingle<int>();
                return new QueryResult<object>() {
                    Items = logs,
                    TotalItems = totalItems,
                };
            }
            else {
                var result = await connection.QueryAsync<object>(sql, (object)query.SqlParameters);
                return new QueryResult<object> { Items = result.ToList() };
            }
        }

        public async Task<QueryResult<T>> QueryAsync<T>(EntityQuery query, string sqlQuery, string sqlCountQuery, string orderBy = null) {

            using var connection = Context.Connection;

            var sql = Context.SetTablePrefix(query.GetSqlQuery(sqlQuery, orderBy));
            
            if (query.RequireTotalCount) {
                var results = await connection.QueryMultipleAsync(sql, (object)query.SqlParameters);
                var logs = results.Read<T>().ToList();
                var totalItems = results.ReadSingle<int>();
                return new QueryResult<T> {
                    Items = logs,
                    TotalItems = totalItems,
                };
            }
            else {
                var result = await connection.QueryAsync<T>(sql, (object)query.SqlParameters);
                return new QueryResult<T> { Items = result.ToList() };
            }
        }

        //public async Task<dynamic> FirstAsync(string sql, object parameters = null) {
        //    using var connection = Context.Connection;
        //    return await connection.QueryFirstOrDefaultAsync(sql, parameters);
        //}

        //public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters) {
        //    using var connection = Context.Connection;
        //    return await connection.QueryAsync(sql, parameters);
        //}
    }
}
