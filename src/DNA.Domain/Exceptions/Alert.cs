using DNA.Domain.Models;
using DNA.Domain.Services.Communication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Domain.Exceptions {
    public static class LoggerEx {

        public static Alert Log(this ILogger logger, string m, params (string name, object value)[] values) {

            var list = values.ToList();
            if (list.Count > 0) {
                NLog.LogManager.Configuration.Variables["EntityName"] = $"{list[0].name}";
                NLog.LogManager.Configuration.Variables["EntityKey"] = $"{list[0].value}";
                list.RemoveAt(0);
            }
            var f = new Alert(new KeyValue(m), values.ToArray());
            logger.LogInformation(message: f.Message);
            return f;
        }
        public static Alert LogWarning(this ILogger logger, string m, params (string name, object value)[] values) {

            var list = values.ToList();
            if (list.Count > 0) {
                NLog.LogManager.Configuration.Variables["EntityName"] = $"{list[0].name}";
                NLog.LogManager.Configuration.Variables["EntityKey"] = $"{list[0].value}";
                list.RemoveAt(0);
            }
            var f = new Alert(new KeyValue(m), values.ToArray());
            logger.LogWarning(message: f.Message);
            return f;
        }
        public static Alert LogInformation(this ILogger logger, KeyValue alertCode, IModel model, params (string name, object value)[] values) {
            Alert f;
            if (model != null) {
                NLog.LogManager.Configuration.Variables["EntityName"] = $"{model.GetType().Name}";
                NLog.LogManager.Configuration.Variables["EntityKey"] = $"{model.Id}";
                f = new Alert(alertCode: alertCode, values);
                logger.LogInformation(message: f.Message);
            }
            else {
                var list = values.ToList();
                if (list.Count > 0) {
                    NLog.LogManager.Configuration.Variables["EntityName"] = $"{list[0].name}";
                    NLog.LogManager.Configuration.Variables["EntityKey"] = $"{list[0].value}";
                    list.RemoveAt(0);
                }
                f = new Alert(alertCode: alertCode, values);
                logger.LogInformation(message: f.Message);
            }
            NLog.LogManager.Configuration.Variables["EntityName"] = null;
            NLog.LogManager.Configuration.Variables["EntityKey"] = null;
            return f;
        }

        public static Alert Log(this ILogger logger, Response response, params (string name, object value)[] values) {
            if (response.Success) {
                return LogInformation(logger, AlertCodes.GeneralInfo, null, values.ToArray());
            }
            else {
                var ex = response.GetException();
                logger.LogError(ex, values);
                return (ex != null && ex is Alert) ? ex as Alert : new Alert(AlertCodes.GeneralError, ex, values);
            }
        }

        public static Alert Log(this ILogger logger, KeyValue alertCode, Response response, params (string name, object value)[] values) {
            if (response.Success) {
                return LogInformation(logger, alertCode, null, values.ToArray());
            }
            else {
                var ex = response.GetException();
                logger.LogError(ex, values);
                return (ex != null && ex is Alert) ? ex as Alert : new Alert(AlertCodes.GeneralError, ex, values);
            }
        }

        public static Alert LogInformation(this ILogger logger, KeyValue alertCode, params (string name, object value)[] values) {
            return LogInformation(logger, alertCode, null, values.ToArray());
        }

        public static Alert LogWarning(this ILogger logger, KeyValue alertCode, IModel model, params (string name, object value)[] values) {
            if (model != null) {
                NLog.LogManager.Configuration.Variables["EntityName"] = $"{model.GetType().Name}";
                NLog.LogManager.Configuration.Variables["EntityKey"] = $"{model.Id}";
            }
            else {
                var list = values.ToList();
                if (list.Count > 0) {
                    NLog.LogManager.Configuration.Variables["EntityName"] = $"{list[0].name}";
                    NLog.LogManager.Configuration.Variables["EntityKey"] = $"{list[0].value}";
                    list.RemoveAt(0);
                }
            }
            var f = new Alert(alertCode, values);
            logger.LogWarning(message: f.Message);
            NLog.LogManager.Configuration.Variables["EntityName"] = null;
            NLog.LogManager.Configuration.Variables["EntityKey"] = null;
            return f;
        }

        public static Alert LogWarning(this ILogger logger, KeyValue alertCode, params (string name, object value)[] values) {
            return LogWarning(logger, alertCode, null, values);
        }

        public static Alert LogError(this ILogger logger, KeyValue alertCode, IModel model, Exception ex, params (string name, object value)[] values) {
            var list = values.ToList();
            if (model != null) {
                NLog.LogManager.Configuration.Variables["EntityName"] = $"{model.GetType().Name}";
                NLog.LogManager.Configuration.Variables["EntityKey"] = $"{model.Id}";
            }
            else {
                if (list.Count > 0) {
                    NLog.LogManager.Configuration.Variables["EntityName"] = $"{list[0].name}";
                    NLog.LogManager.Configuration.Variables["EntityKey"] = $"{list[0].value}";
                    list.RemoveAt(0);
                }
            }
            Alert f;
            if (ex is Alert) {
                logger.LogError(ex, ex.Message);
                f = ex as Alert;
            }
            else {
                f = new Alert(alertCode ?? AlertCodes.UndefinedError, ex, list.ToArray());
                logger.LogError(f, message: f.Message);
            }

            NLog.LogManager.Configuration.Variables["EntityName"] = null;
            NLog.LogManager.Configuration.Variables["EntityKey"] = null;

            return f;
        }
        public static Alert LogError(this ILogger logger, KeyValue alertCode, Exception ex, params (string name, object value)[] values) {
            return LogError(logger, alertCode, null, ex, values.ToArray());
        }

        public static Alert LogError(this ILogger logger, Exception ex, params (string name, object value)[] values) {
            return LogError(logger, (KeyValue)null, (IModel)null, ex, values.ToArray());
        }
    }

    public class Alert : Exception {
        public int Code { get; set; }

        //public Alert(string m, params (string name, object value)[] values)
        //    : base(message: $"{m} | {string.Join(' ', values.Select(_ => $"[{_.name}={_.value}]"))}".Trim().Trim('|'), innerException: null) {
        //    Code = 0;
        //}

        public Alert(KeyValue alertCode, params (string name, object value)[] values)
            : base($"{alertCode.Value} | {string.Join(' ', values.Select(_ => $"[{_.name}={_.value}]"))}".Trim().Trim('|')) {
            Code = alertCode.Key;
        }

        public Alert(KeyValue alertCode, string errorMessage, params (string name, object value)[] values)
            : base($"{errorMessage} | {alertCode?.Value} | {string.Join(' ', values.Select(_ => $"[{_.name}={_.value}]"))}".Trim().Trim('|'), new Exception(errorMessage)) {
            Code = alertCode.Key;
        }

        public Alert(KeyValue alertCode, Exception inner, params (string name, object value)[] values)
            : base($"{inner?.Message} | {alertCode?.Value} | {string.Join(' ', values.Select(_ => $"[{_.name}={_.value}]"))}".Trim().Trim('|'), inner) {
            Code = alertCode.Key;
        }

        public Alert(KeyValue alertCode, string errorMessage, string errorDesc, params (string name, object value)[] values)
            : base($"{errorMessage} | {alertCode.Value} | {string.Join(' ', values.Select(_ => $"[{_.name}={_.value}]"))}".Trim().Trim('|'),
                  new Exception($"{errorMessage} {errorDesc}")) {
            Code = alertCode.Key;
        }

        public Alert(KeyValue alertCode) : base(alertCode.Value) {
            Code = alertCode.Key;
        }

        public Alert(KeyValue alertCode, Exception inner) : base(alertCode.Value, inner) {
            Code = alertCode.Key;
        }
    }
}
