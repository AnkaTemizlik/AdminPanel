using DNA.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {

    public interface IRestService {
        void AddHeader(string name, string value);
        Task<Response<T>> ExecuteAsync<T>(HttpMethod method, string endpoint, object body = null, string contentType = null);
        Task<Response<string>> ExecuteAsync(HttpMethod method, string endpoint, object body = null, string contentType = null);
        T Get<T>(string urlPath, Dictionary<string, string> parameters);
        Task<Response<T>> GetAsync<T>(string endpoint);
        Task<Response<string>> GetAsync(string endpoint, string contentType = null);
        IRestService Init(Uri baseUrl, string contentType = "application/json");
        Task<Response<T>> PostAsync<T>(string endpoint, object body, string contentType = null);
        Task<Response<string>> PostAsync(string endpoint, object body, string contentType = null);
        void SetToken(string value, string name = "Authorization");
    }
}
 