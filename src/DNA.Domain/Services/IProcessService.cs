using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IProcessService {
        Task<int> ExecuteAsync(string sql, object parameters = null);
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
        Task<dynamic> FirstAsync(string sql, object parameters = null);
        Task<T> FirstAsync<T>(string sql, object parameters = null);
        Task<T> GetAsync<T>(int id) where T : class;
        Task<int> InsertAsync<T>(T row) where T : class;
        Task<bool> UpdateAsync<T>(T row) where T : class;

    }
}


