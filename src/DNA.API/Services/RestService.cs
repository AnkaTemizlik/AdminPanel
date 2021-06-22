using AutoMapper;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IRestService), Lifetime.Scoped)]
    public class RestService : IRestService {

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<RestService> _logger;

        string _contentType;
        Uri _baseUrl;
        RestClient _client;

        public RestService(IConfiguration configuration, IMapper mapper, ILogger<RestService> logger) {
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        public IRestService Init(Uri baseUrl, string contentType = "application/json") {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _contentType = contentType;
            _client = new RestClient(_baseUrl) {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            _client.UseSerializer<RestSharp.Serializers.NewtonsoftJson.JsonNetSerializer>();
            return this;
        }

        public void SetToken(string value, string name = "Authorization") {
            _client.AddDefaultParameter(name, value, ParameterType.HttpHeader);
        }

        public async Task<Response<T>> GetAsync<T>(string endpoint) {
            return await ExecuteAsync<T>(HttpMethod.Get, endpoint);
        }

        public async Task<Response<T>> PostAsync<T>(string endpoint, object body) {
            return await ExecuteAsync<T>(HttpMethod.Post, endpoint, body);
        }

        public T Get<T>(string urlPath, Dictionary<string, string> parameters) {
            if (_baseUrl == null || _client == null) {
                throw new Exception($"Run 'Init' before execution.");
            }

            var request = new RestRequest(urlPath, Method.GET);

            foreach (var item in parameters)
                request.AddQueryParameter(item.Key, item.Value);

            var queryResult = _client.Execute(request);

            if (queryResult == null)
                throw new Exception("RestClient execution failed. Host:" + _baseUrl + " path:" + urlPath);

            if (!queryResult.IsSuccessful) {
                if (queryResult.ErrorException != null) {
                    throw queryResult.ErrorException;
                }
                else if (!string.IsNullOrWhiteSpace(queryResult.ErrorMessage)) {
                    throw new Exception(queryResult.ErrorMessage);
                }
                else {
                    throw new Exception("Unknown error result. Host:" + _baseUrl + " path:" + urlPath);
                }
            }

            var result = JsonConvert.DeserializeObject<T>(queryResult.Content);

            return result;
        }

        public async Task<Response<T>> ExecuteAsync<T>(HttpMethod method, string endpoint, object body = null) {
            if (_baseUrl == null || _client == null) {
                throw new Exception($"Run 'Init' before execution.");
            }

            var request = new RestRequest(endpoint, _mapper.Map<Method>(method), DataFormat.Json);
            request.AddHeader("Content-Type", _contentType);

            if (body != null)
                request.AddJsonBody(body);
                //request.AddJsonBody(body is string ? body : JsonConvert.SerializeObject(body));

            var restResponse = _client.Execute<T>(request);
            var queryResult = new Response<T> {
                Code = (int)restResponse.StatusCode,
                Success = restResponse.StatusCode == System.Net.HttpStatusCode.OK,
                Comment = restResponse.StatusDescription,
                Message = restResponse.ErrorException?.Message,
                Resource = restResponse.Data
            };

            return await Task.FromResult(queryResult);
        }
    }
}
