using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Repositories {
    public interface ILogRepository {
        Task<QueryResult<Log>> ListAsync(Query query);
        Task<Log> GetByIdAsync(int id);
    }
}
