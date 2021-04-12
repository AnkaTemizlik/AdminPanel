using DNA.Domain.Repositories;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Extentions;
using AutoMapper;
using System.Reflection;
using Newtonsoft.Json;
using System.Dynamic;

namespace DNA.API.Services {

    [Service(typeof(IEntityService), Lifetime.Scoped)]
    public class EntityService : IEntityService {

        readonly IEntityRepository _entityRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IMapper _mapper;
        readonly IConfiguration _configuration;
        public EntityService(IConfiguration configuration, IEntityRepository entityRepository, IProcessRepository processRepository, IMapper mapper) {
            _entityRepository = entityRepository;
            _processRepository = processRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        private readonly object _lock = new object();

        public bool TryFindType(string typeName, out Type type) {
            lock (typeCache) {
                if (!typeCache.TryGetValue(typeName ?? "", out type)) {
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                        type = a.GetType(typeName);
                        if (type != null)
                            break;
                    }
                    typeCache[typeName] = type; // perhaps null
                }
            }
            return type != null;
        }

        //Type GetEntityType(string name) => Type.GetType(_configuration[$"ScreenConfig:Screens:{name}:assembly"]) ?? Type.GetType("System.Object");

        public async Task<QueryResult<object>> ListAsync(EntityQuery query) {
            if (!TryFindType(_configuration[$"ScreenConfig:Screens:{query.Name}:assembly"], out Type type))
                type = typeof(object);
            return await _entityRepository.QueryAsync(type, query, GetSelectQuery(query.Name), null, GetOrderBy(query.Name));
        }

        public async Task<QueryResult<T>> ListAsync<T>(EntityQuery query) {
            return await _entityRepository.QueryAsync<T>(query, GetSelectQuery(query.Name), null, GetOrderBy(query.Name));
        }

        string GetOrderBy(string name) => _configuration[$"ScreenConfig:Queries:{name}:SelectQuery:OrderBy"];

        string GetSelectQuery(string name) {
            var selectQuery = _configuration[$"ScreenConfig:Queries:{name}:SelectQuery"];

            if (string.IsNullOrWhiteSpace(selectQuery)) {
                var selectQuerySection = _configuration.GetSection($"ScreenConfig:Queries:{name}:SelectQuery");
                var lines = selectQuerySection.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new ArgumentNullException("selectQuery", "Gerekli olan sorgu tanımlanmadı.");
                selectQuery = string.Join(Environment.NewLine, lines);
            }
            return selectQuery;
        }

        public async Task<dynamic> GetByIdAsync(string name, string id) {
            return await GetByIdAsync(name, new { Id = id });
        }

        public async Task<dynamic> GetByIdAsync(string name, object parameters) {
            var selectByIdQuery = _configuration[$"ScreenConfig:Queries:{name}:SelectById"];
            if (string.IsNullOrWhiteSpace(selectByIdQuery)) {
                var selectByIdQuerySection = _configuration.GetSection($"ScreenConfig:Queries:{name}:SelectById");
                var lines = selectByIdQuerySection.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new ArgumentNullException("selectByIdQuery", "Gerekli olan sorgu tanımlanmadı.");
                selectByIdQuery = string.Join(Environment.NewLine, lines);
            }
            return await _entityRepository.GetByIdAsync<dynamic>(selectByIdQuery, parameters);
        }

        public async Task<int> InsertAsync(string name, Model model) {
            var insertQuery = _configuration[$"ScreenConfig:Queries:{name}:InsertQuery"];
            var section = _configuration.GetSection($"ScreenConfig:Queries:{name}:InsertQuery");
            if (section.Exists()) {
                var lines = section.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new Exception("InsertQuery is not defined.");
                var sql = string.Join(Environment.NewLine, lines);
                if (sql.Contains("{Fields}")) {
                    var data = model.GetType().GetProperties()
                        .Where(_ => _.GetCustomAttribute<Dapper.Contrib.Extensions.KeyAttribute>() == null)
                        .Where(_ => _.GetCustomAttribute<Dapper.Contrib.Extensions.ComputedAttribute>() == null)
                        .Select(_ => _.Name);
                    //var data = (System.Dynamic.ExpandoObject)model;
                    sql = sql.Replace("{Fields}", string.Join(",", data.Select(name => $"[{name}]")));
                    sql = sql.Replace("{Parameters}", string.Join(",", data.Select(name => $"@{name}")));
                }
                return await _processRepository.ExecuteAsync(sql, JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(model)));
            }
            else {
                return await _processRepository.InsertAsync(model);
            }
            //return await InsertAsync(name, JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(model)));
        }

        public async Task<int> InsertAsync(string name, dynamic model) {
            var insertQuery = _configuration[$"ScreenConfig:Queries:{name}:InsertQuery"];
            var section = _configuration.GetSection($"ScreenConfig:Queries:{name}:InsertQuery");
            if (section.Exists()) {
                var lines = section.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new Exception("InsertQuery is not defined.");
                var sql = string.Join(Environment.NewLine, lines);
                if (sql.Contains("{Fields}")) {
                    var data = (System.Dynamic.ExpandoObject)model;
                    sql = sql.Replace("{Fields}", string.Join(",", data.Select(_ => $"[{_.Key}]")));
                    sql = sql.Replace("{Parameters}", string.Join(",", data.Select(_ => $"@{_.Key}")));
                }
                return await _processRepository.ExecuteAsync(sql, model);
            }
            else {
                throw new Exception("InsertQuery is not defined.");
            }
        }

        public async Task<int> UpdateAsync(string name, string id, Model model) {
            return await UpdateAsync(name, id, JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(model)));
        }

        public async Task<int> UpdateAsync(string name, string id, dynamic model) {
            var section = _configuration.GetSection($"ScreenConfig:Queries:{name}:UpdateQuery");
            if (section.Exists()) {
                var lines = section.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new Exception("UpdateQuery is not defined.");
                var sql = string.Join(Environment.NewLine, lines);
                if (sql.Contains("{Fields}")) {
                    var data = (System.Dynamic.ExpandoObject)model;
                    sql = sql.Replace("{Fields}", string.Join(",", data.Select(_ => $"[{_.Key}] = @{_.Key}")));

                    if (sql.Contains("{InsertingFields}"))
                        sql = sql.Replace("{InsertingFields}", string.Join(",", data.Select(_ => $"[{_.Key}]")));
                    if (sql.Contains("{InsertingParameters}"))
                        sql = sql.Replace("{InsertingParameters}", string.Join(",", data.Select(_ => $"@{_.Key}")));

                }
                model.Id = id;
                return await _processRepository.ExecuteAsync(sql, model);
            }
            else {
                throw new Exception("UpdateQuery is not defined.");
            }
        }

        public async Task<int> DeleteAsync(string name, Model model) {
            return await DeleteAsync(name, JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(model)));
        }

        public async Task<int> DeleteAsync(string name, dynamic model) {
            var section = _configuration.GetSection($"ScreenConfig:Queries:{name}:DeleteQuery");
            if (section.Exists()) {
                var lines = section.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new Exception("DeleteQuery is not defined.");
                return await _processRepository.ExecuteAsync(string.Join(Environment.NewLine, lines), model);
            }
            else {
                throw new Exception("DeleteQuery is not defined.");
            }
        }

        public async Task<dynamic> GetEntityModelAsync(EntityQuery entityQuery) {
            var selectQuery = _configuration[$"ScreenConfig:Queries:{entityQuery.Name}:SelectQuery"];
            if (string.IsNullOrWhiteSpace(selectQuery)) {
                var selectQuerySection = _configuration.GetSection($"ScreenConfig:Queries:{entityQuery.Name}:SelectQuery");
                var lines = selectQuerySection.GetSection("Lines").Get<string[]>();
                if (lines == null || lines.Length == 0)
                    throw new ArgumentNullException("selectQuery", "Gerekli olan sorgu tanımlanmadı.");
                selectQuery = string.Join(Environment.NewLine, lines);
            }

            var queryResult = await _entityRepository.QueryAsync(entityQuery, selectQuery, null);
            return queryResult.Items.FirstOrDefault();
        }

        public async Task<Dictionary<string, IEnumerable<dynamic>>> CardListAsync(CardsQuery cardQuery) {
            var result = new Dictionary<string, IEnumerable<dynamic>>();
            if (cardQuery == null)
                throw new ArgumentNullException("cardQuery", "Gerekli olan sorgular tanımlanmadı.");
            if (cardQuery.Names.Count == 0)
                throw new ArgumentNullException("cardQuery", "Gerekli olan sorgular tanımlanmadı.");

            foreach (var name in cardQuery.Names) {
                var query = _configuration[$"ScreenConfig:CardQueries:{name}"];
                if (string.IsNullOrWhiteSpace(query)) {
                    var querySection = _configuration.GetSection($"ScreenConfig:CardQueries:{name}");
                    var lines = querySection.GetSection("Lines").Get<string[]>();
                    if (lines == null || lines.Length == 0) {
                        result.Add(name, new List<dynamic> { new { } });
                        continue;
                    }
                    else {
                        query = string.Join(Environment.NewLine, lines);
                    }
                }
                var list = await _entityRepository.QueryAsync(query);
                result.Add(name, list);
            }
            return result;
        }

    }
}
