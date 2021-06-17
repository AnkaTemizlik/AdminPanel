using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using DNA.Domain;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Repositories;
using DNA.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Persistence.Repositories {

    [Service(typeof(IProcessRepository), Lifetime.Scoped)]
    public class ProcessRepository : BaseRepository, IProcessRepository {
        public ProcessRepository(IAppDbContext context) : base(context) { }

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

    }
}
