using DNA.API.Models;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IValuerService), Lifetime.Scoped)]
    public class ValuerService : IValuerService {
        Dictionary<string, object> _models = new Dictionary<string, object>();

        const string _fullNamePattern = @"{(?<fullName>[\w\.]+)}";
        const string _fieldInFullNamePattern = @"(?<field>[\w]+)";
        const string _singleFieldNamePattern = @"{(?<field>)[\w]+}";

        public void SetCurrentModel(params object[] models) {
            foreach (var model in models) {
                if (model == null)
                    continue;
                var name = model.GetType().Name;
                if (name == nameof(ApplicationUser))
                    Add("User", model);
                Add(name, model);
            }
        }

        void Add(string name, object model) {
            if (_models.ContainsKey(name)) {
                _models[name] = model;
            }
            else {
                _models.Add(name, model);
            }
        }

        public TValue GetByType<TValue>(string val) {
            try {
                if (string.IsNullOrWhiteSpace(val))
                    return default;
                var textValue = Get(val);
                if (string.IsNullOrWhiteSpace(textValue))
                    return default;
                return (TValue)Convert.ChangeType(textValue, typeof(TValue));
            }
            catch (Exception ex) {
                throw new Exception($"appsettings: Changing '{val}' value to '{typeof(TValue).Name}' type failed.", ex);
            }
        }

        public string Get(string source) {
            if (string.IsNullOrWhiteSpace(source))
                return source;

            if (Regex.IsMatch(source, _singleFieldNamePattern))
                return source;

            var res = Regex.Replace(source, _fullNamePattern, (m) => {
                var val = m.Value;
                var matchs = Regex.Matches(val, _fieldInFullNamePattern);

                if (matchs.Count == 0)
                    return val;

                var objectName = matchs[0].Groups["field"].Value;
                var fieldName = matchs[1].Groups["field"].Value;
                if (fieldName == null)
                    return val;

                if (!_models.ContainsKey(objectName))
                    return val;

                var propText = GetValue(_models[objectName], fieldName) ?? val;

                return propText;

            });

            return res;
        }

        string GetValue(object model, string fieldName) {
            var property = model.GetType().GetProperty(fieldName);
            if (property == null)
                return null;

            var propValue = property.GetValue(model);
            if (propValue == null)
                return "";

            var propText = propValue.ToString();
            return propText;
        }

        public string GetByObject(object model, string source) {
            if (model == null)
                return source;
            if (string.IsNullOrWhiteSpace(source))
                return source;

            var result = Regex.Replace(source, _singleFieldNamePattern, (m) => {
                var val = m.Value;
                var fieldName = m.Groups["field"].Value;
                if (string.IsNullOrWhiteSpace(fieldName))
                    return val;

                return GetValue(model, fieldName) ?? val;
            });

            return result;
        }

        //public string Get(IModel model, string source) {
        //    if (model == null)
        //        return source;
        //    if (string.IsNullOrWhiteSpace(source))
        //        return source;

        //    var result = Regex.Replace(source, _singleFieldNamePattern, (m) => {
        //        var val = m.Value;
        //        var fieldName = m.Groups["field"].Value;
        //        if (string.IsNullOrWhiteSpace(fieldName))
        //            return val;

        //        return GetValue(model, fieldName) ?? val;
        //    });

        //    return result;
        //}

        //public string GetByModelType<TModel>(string source) {
        //    var objectName = typeof(TModel).Name;

        //    if (!_models.ContainsKey(objectName))
        //        return source;

        //    if (string.IsNullOrWhiteSpace(source))
        //        return source;

        //    var result = Regex.Replace(source, _singleFieldNamePattern, (m) => {
        //        var val = m.Value;
        //        var fieldName = m.Groups["field"].Value;
        //        if (string.IsNullOrWhiteSpace(fieldName))
        //            return val;

        //        return GetValue(_models[objectName], fieldName) ?? val;
        //    });

        //    return result;
        //}

    }
}
