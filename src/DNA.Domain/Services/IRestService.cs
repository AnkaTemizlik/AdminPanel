using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DNA.Domain.Services {

    public interface IRestService {
        System.Threading.Tasks.Task<Communication.Response<T>> ExecuteAsync<T>(HttpMethod method, string endpoint, object body = null);
        T Get<T>(string urlPath, Dictionary<string, string> parameters);
        System.Threading.Tasks.Task<Communication.Response<T>> GetAsync<T>(string endpoint);
        IRestService Init(Uri baseUrl, string contentType = "application/json");
        System.Threading.Tasks.Task<Communication.Response<T>> PostAsync<T>(string endpoint, object body);
        void SetToken(string value, string name = "Authorization");
    }
}
 