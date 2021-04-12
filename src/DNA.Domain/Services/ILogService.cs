using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface ILogService {
        Task<QueryResult<Log>> ListAsync(Query query);
        Task<Log> GetByIdAsync(int id);
        Task<Dictionary<string, string>> ReadFileLogAsync();
    }
}
