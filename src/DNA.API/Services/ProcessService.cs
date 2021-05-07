using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Repositories;
using DNA.Domain.Services;
using Newtonsoft.Json;
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

        System.Dynamic.ExpandoObject ParseParams(object parameters) {
            object p = parameters;
            if (parameters != null) {
                p = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(JsonConvert.SerializeObject(parameters));
            }
            return p as System.Dynamic.ExpandoObject;
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null) {
            return await _processRepository.ExecuteAsync(sql, ParseParams(parameters));
        }

        public async Task<dynamic> FirstAsync(string sql, object parameters = null) {
            return await _processRepository.FirstAsync(sql, ParseParams(parameters));
        }

        public async Task<T> FirstAsync<T>(string sql, object parameters = null) {
            
            return await _processRepository.FirstAsync<T>(sql, ParseParams(parameters));
        }

        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object parameters = null) {
            return await _processRepository.QueryAsync(sql, ParseParams(parameters));
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null) {
            return await _processRepository.QueryAsync<T>(sql, ParseParams(parameters));
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
