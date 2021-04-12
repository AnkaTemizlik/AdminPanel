using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace DNA.Domain.Models {

    public interface IBaseModel {
        dynamic ToDynamic();
        string ToJson();
        void FromDynamic(dynamic dyn);
        string Format(string text);
    }

    public abstract class BaseModel {

        public virtual void FromDynamic(dynamic dyn) { }

        public virtual dynamic ToDynamic() {
            var converter = new ExpandoObjectConverter();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            var obj = JsonConvert.DeserializeObject<dynamic>(json, converter);
            return obj;
        }

        public virtual string Format(string text) {
            return Format(this, text);
        }

        //[Obsolete("Use IValuerService.Get method.")]
        public static string Format(object model, string text) {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            var result = Regex.Replace(text, @"{(?<field>\w+)}", (m) => {
                var field = m.Groups["field"].Value;
                if (field == null)
                    return m.Value;
                var property = model.GetType().GetProperty(field);
                if (property == null)
                    return m.Value;
                var propValue = property.GetValue(model) ?? "";
                return propValue.ToString();
            });
            return result;
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(this);
        }
    }

    public interface IModel : IBaseModel {
        int Id { get; set; }
        DateTime CreationTime { get; set; }
        DateTime UpdateTime { get; set; }
    }

    public abstract class Model : BaseModel, IModel {

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column]
        public virtual int Id { get; set; }

        [Write(false)]
        [Computed]
        [System.ComponentModel.DataAnnotations.Schema.Column]
        public virtual DateTime CreationTime { get; set; } = DateTime.Now;

        [System.ComponentModel.DataAnnotations.Schema.Column]
        public virtual DateTime UpdateTime { get; set; } = DateTime.Now;
       
    }
}