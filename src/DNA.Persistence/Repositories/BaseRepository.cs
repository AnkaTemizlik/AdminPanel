using Dapper;
using DNA.Persistence.Contexts;
using DNA.Domain;
using DNA.Domain.Models;
using System;
using DNA.Domain.Repositories;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace DNA.Persistence.Repositories {
    public abstract class BaseRepository : IProcessRepository {

        protected readonly IAppDbContext Context;

        public BaseRepository(IAppDbContext context) {
            Context = context;
        }

        public async Task<int> ExecuteAsync(string sql, dynamic parameters) {
            var dynParams = new DynamicParameters();
            if (parameters != null)
                dynParams.AddDynamicParams(parameters);
            using var connection = Context.Connection;
            return await connection.QueryFirstOrDefaultAsync<int>(Context.SetTablePrefix(sql), dynParams);
        }

        //public async Task<int> ExecuteAsync(string sql, object parameters = null) {
        //    var dynParams = new DynamicParameters();
        //    dynParams.AddDynamicParams(parameters);
        //    using var connection = Context.Connection;
        //    return await connection.ExecuteAsync(sql, dynParams);
        //}

        public async Task<dynamic> FirstAsync(string sql, object parameters = null) {
            using var connection = Context.Connection;
            return await connection.QueryFirstOrDefaultAsync(Context.SetTablePrefix(sql), parameters);
        }
        public async Task<T> FirstAsync<T>(string sql, object parameters = null) {
            using var connection = Context.Connection;
            return await connection.QueryFirstOrDefaultAsync<T>(Context.SetTablePrefix(sql), parameters);
        }
        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters) {
            using var connection = Context.Connection;
            return await connection.QueryAsync(Context.SetTablePrefix(sql), parameters);
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters) {
            using var connection = Context.Connection;
            return await connection.QueryAsync<T>(Context.SetTablePrefix(sql), parameters);
        }

        public async Task<T> GetAsync<T>(int id) where T : class {
            using var connection = Context.Connection;
            var single = connection.Get<T>(id);
            return await Task.FromResult(single);
        }

        public async Task<int> InsertAsync<T>(T row) where T : class {
            using var connection = Context.Connection;
            return await connection.InsertAsync<T>(row);
        }

        public async Task<bool> UpdateAsync<T>(T row) where T : class {
            using var connection = Context.Connection;
            return await connection.UpdateAsync<T>(row);
        }

        public async Task<bool> DeleteAsync<T>(T row) where T : class {
            using var connection = Context.Connection;
            return await connection.DeleteAsync<T>(row);
        }

        public void Set(string connectionString) {
            Context.Set(connectionString);
        }

        //protected string CleanCondition(string condition) {
        //    if (condition != null) {
        //        var check = condition.ToLower();
        //        if (check.Contains("update")
        //            || check.Contains("delete")
        //            || check.Contains("insert") || condition.ToUpper().Contains("INSERT")
        //            || check.Contains("begin") || condition.ToUpper().Contains("BEGIN")
        //            || check.Contains("trancate")
        //            || check.Contains("commit") || condition.ToUpper().Contains("COMMIT")
        //            || check.Contains(";")
        //            )

        //            return " 1 = 0 ";
        //    }
        //    return condition;
        //}
    }
}
