using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Repositories;
using DNA.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IProcessService), Lifetime.Scoped)]
    public class ProcessService : IProcessService {
        IProcessRepository _processRepository;
        public ProcessService(IProcessRepository processRepository) {
            _processRepository = processRepository;
        }

        public async Task<int> ExecuteAsync(string sql, object paramaters = null) {
            return await _processRepository.ExecuteAsync(sql, paramaters);
        }

        public async Task<dynamic> FirstAsync(string sql, object paramaters = null) {
            return await _processRepository.FirstAsync(sql, paramaters);
        }

        public async Task<T> FirstAsync<T>(string sql, object paramaters = null) {
            return await _processRepository.FirstAsync<T>(sql, paramaters);
        }

        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object paramaters = null) {
            return await _processRepository.QueryAsync(sql, paramaters);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object paramaters = null) {
            return await _processRepository.QueryAsync<T>(sql, paramaters);
        }

        public async Task<T> GetAsync<T>(int id) where T : class {
            return await _processRepository.GetAsync<T>(id);
        }

        public async Task<int> InsertAsync<T>(T row) where T : class {
            return await _processRepository.InsertAsync<T>(row);
        }

        public async Task<bool> UpdateAsync<T>(T row) where T : class {
            return await _processRepository.UpdateAsync<T>(row);
        }
        //public async Task<bool> BulkMergeAsync<T>(List<T> rows) where T : class {
        //    return await _processRepository.BulkMergeAsync<T>(rows);
        //}
    }
}
