using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IEntityService {
        Task<QueryResult<object>> ListAsync(EntityQuery query);
        Task<QueryResult<T>> ListAsync<T>(EntityQuery query);
        Task<int> InsertAsync(string name, dynamic model);
        Task<int> InsertAsync(string name, Model model);
        Task<int> UpdateAsync(string name, string id, dynamic model);
        Task<int> UpdateAsync(string name, string id, Model model);
        Task<int> DeleteAsync(string name, dynamic model);
        Task<int> DeleteAsync(string name, Model model);
        Task<dynamic> GetEntityModelAsync(EntityQuery entityQuery);
        Task<Dictionary<string, IEnumerable<dynamic>>> CardListAsync(CardsQuery cardQuery);
        Task<dynamic> GetByIdAsync(string name, string id);
        Task<dynamic> GetByIdAsync(string name, object parameters);
    }
}
