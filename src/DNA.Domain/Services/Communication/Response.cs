using DNA.Domain.Exceptions;
using DNA.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DNA.Domain.Services.Communication {

    /// <summary>
    /// API işlemlerinin geri dönüş mesajıdır. Başarılı yada başarısız durumları için ilgili alanlara bakın.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Response {

        /// <summary>
        /// İşlem başarılı ise <code>true</code> döner.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// İşlem başarısız ise başarısız olma nedenini verir.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// İşlemlerin başarısız olma nedeni için gerekli olabilecek açıklamaları içerir.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// İşlem başarılı ise 0, başarısız ise ilgili hata kodunu verir. Daha önce tanımlanmamış bir hata ise yine 0 verir.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Hata bilgisinin teknik detaylarını verir. Hatanın nedenini bilmiyorsanız, sorunun çözümü için bu detayları sistem yöneticisine iletin.
        /// </summary>
        public List<string> Details { get; set; }

        /// <summary>
        /// İşlem başarılı ise isteninlen veriyi bu alandan edinin. 
        /// </summary>
        public object Resource { get; set; }

        int _level = 1;
        Exception _exception;

        public Response() {
            Success = true;
        }

        public Response(string error) {
            Success = false;
            Message = error;
        }

        public Response(Exception ex) {

            if (_exception != null)
                throw new Exception("Exception already added.");

            _exception = ex;

            Success = ex == null;
            AddMessages(ex);
        }

        public Response(Alert ex) {

            if (_exception != null)
                throw new Exception("Alert already added.");

            _exception = ex;

            Success = ex == null;
            Code = ex.Code;
            AddMessages(ex);
        }

        public Response(object data) {
            Success = data != null;
            Resource = data;
            Message = data == null ? "Data is empty" : null;
        }

        public Response(bool isSuccessful, string errorCode, string message, string errorDesc) {
            Success = isSuccessful;
            Message = message;

            int.TryParse(errorCode, out int code);

            if (errorDesc != null) {
                if (errorDesc.Contains("\r\n")) {
                    errorDesc = Regex.Replace(errorDesc, @"\r\n<ErrorHeader>[^<>]+</ErrorHeader>\r\n|<Hata>\r\n", "");
                    errorDesc = Regex.Replace(errorDesc, @"NetOpenX", "\nNetOpenX");
                    errorDesc = Regex.Replace(errorDesc, "\r\n", "\n");
                    Details = errorDesc.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (Message == null) {
                        Message = Details.FirstOrDefault(_ => _.StartsWith("(Acik)")) ?? Details.FirstOrDefault(_ => _.StartsWith("Detay : "));
                        if (Message != null)
                            Message = Message.Replace("Detay :", "").Trim();
                    }
                    if (Message != null && Message.StartsWith("Hata Kodu")) {
                        if (Details.Count > 2)
                            Message = Details[2];
                        Message = Message.Replace("Detay :", "").Trim();
                        if (Message.StartsWith("Hata Kodu") && Details.Count > 3)
                            Message = Details[3];
                        Message = Message.Replace("Detay :", "").Trim();
                        if (Message.StartsWith("Hata Kodu") && Details.Count > 4)
                            Message += ". " + Details[4];
                        Message = Message.Replace("Detay :", "").Trim();
                    }
                    errorCode = Details.FirstOrDefault(_ => _.StartsWith("Hata Kodu :"));
                    if (!string.IsNullOrWhiteSpace(errorCode))
                        int.TryParse(errorCode.Replace("Hata Kodu : ", ""), out code);
                }
                if (string.IsNullOrWhiteSpace(Message))
                    Message = Details != null ? string.Join(". ", Details.Where(_ => !_.StartsWith("Detay") && !_.StartsWith("Hata Kodu"))) : $"{errorDesc}";
            }
            if (code > 0)
                Code = code + 9000;
        }

        void AddMessages(Exception ex) {

            var p = AppDomain.CurrentDomain.BaseDirectory.Split("\\bin\\")[0];
            var s = @"D:\Develop\DNA\Source";

            if (string.IsNullOrWhiteSpace(Message)) {
                Message = ex.Message.Replace(p, string.Empty).Replace(s, "");
            }
            else if (string.IsNullOrWhiteSpace(Comment)) {
                Comment = ex.Message.Replace(p, string.Empty).Replace(s, "");
            }

            Details ??= new List<string>();
            Details.Add(_level + ".Message: " + ex.Message.Replace(p, "").Replace(s, ""));
            Details.Add(_level + ".Type   : " + ex.GetType().Name);
            Details.Add(_level + ".Stack  : " + ex.StackTrace?.Replace(p, "").Replace(s, ""));

            _level++;

            if (ex.InnerException != null) {
                AddMessages(ex.InnerException);
            }
        }

        public Exception GetException() {
            if (_exception != null)
                return _exception;
            _exception = Success
                ? null
                : new Exception($"{Message} {Comment}", new Exception(string.Join(Environment.NewLine, Details ?? new List<string>())));
            return _exception;
        }
    }

    /// <summary>
    /// İstenilen liste verisi sayfalanmış ise toplam kayıt sayısı ile birlikte sonuç döner.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class QueryResultResponse<T> : Response<T> {
        public new QueryResult<T> Resource { get; set; } = new QueryResult<T>();
        public QueryResultResponse() {

        }
        public QueryResultResponse(QueryResult<T> resource) {
            Resource = resource;
        }

        public QueryResultResponse(bool success, string errorCode, string message, string errorDesc)
            : base(success, errorCode, message, errorDesc) {
        }

        public QueryResultResponse(Response response)
            : base(response) {
        }
    }

    /// <summary>
    /// PI işlemlerinin geri dönüş mesajıdır. Başarılı yada başarısız durumları için ilgili alanlara bakın.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Response<T> : Response {
        public new T Resource { get; set; }

        public Response() {
            Success = true;
        }
        public Response(T resource) {
            Success = true;
            Resource = resource;
        }
        public Response(string error) {
            Success = false;
            Message = error;
            Resource = default;
        }

        public Response(Response response) {
            if (response != null) {
                Success = response.Success;
                Message = response.Message;
                Comment = response.Comment;
                Details = response.Details;
                Code = response.Code;
                Resource = response.Resource == null
                    ? default
                    : (T)Convert.ChangeType(response.Resource, typeof(T));
            }
        }

        public Response(Exception ex) : base(ex) { }

        public Response(Alert ex) : base(ex) { }
        public Response(bool IsSuccessful, string errorCode, string message, string errorDesc)
            : base(IsSuccessful, errorCode, message, errorDesc) {

        }
    }

}
