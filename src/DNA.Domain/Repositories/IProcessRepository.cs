using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Repositories {
    public interface IProcessRepository {
        void Set(string connectionString);
        Task<int> ExecuteAsync(string sql, dynamic parameters );
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
        Task<dynamic> FirstAsync(string sql, object parameters = null);
        Task<T> FirstAsync<T>(string sql, object parameters = null);
        Task<T> GetAsync<T>(int id) where T : class;
        Task<int> InsertAsync<T>(T row) where T : class;
        Task<bool> UpdateAsync<T>(T row) where T : class;
        Task<bool> DeleteAsync<T>(T row) where T : class;

        //Task<bool> BulkMergeAsync<T>(List<T> rows) where T : class;

    }
}
