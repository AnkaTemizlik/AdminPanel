using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
 
namespace DNA.Domain.Repositories {
    public interface IEntityRepository {
        Task<IEnumerable<dynamic>> QueryAsync(string sqlQuery);
        Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery);
        Task<QueryResult<dynamic>> QueryAsync(EntityQuery query, string sqlQuery, string orderBy = null);
        Task<QueryResult<object>> QueryAsync(Type type, EntityQuery query, string sqlQuery, string sqlCountQuery, string orderBy = null);
        Task<QueryResult<T>> QueryAsync<T>(EntityQuery query, string sqlQuery, string sqlCountQuery, string orderBy = null);
        Task<T> GetByIdAsync<T>(string selectQuery, object p);
    }
}
