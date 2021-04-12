using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Repositories;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(ILogService), Lifetime.Scoped)]
    public class LogService : ILogService {

        readonly ILogRepository _logRepository;
        readonly IConfiguration _config;
        public LogService(IEntityRepository entityRepository, ILogRepository logRepository, IConfiguration config) {
            _logRepository = logRepository;
            _config = config;
        }

        public async Task<QueryResult<Log>> ListAsync(Query query) {
            var logs = await _logRepository.ListAsync(query);
            return logs;
        }

        public async Task<Log> GetByIdAsync(int id) {
            var log = await _logRepository.GetByIdAsync(id);
            return log;
        }

        public async Task<Dictionary<string, string>> ReadFileLogAsync() {
            /*
            var allfileTarget = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("allfile");
        allfileTarget.FileName = Path.Combine(logPath, "nlog-all-${shortdate}.log");

        var ownFileTarget = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("ownFile-web");
        allfileTarget.FileName = Path.Combine(logPath, "nlog-own-${shortdate}.log");

        NLog.LogManager.Configuration.Variables["internalLogFile"] = Path.Combine(logPath, "internal-nlog.txt");
        */
            var result = new Dictionary<string, string>();
            var logPath = _config.GetSection("Logging").GetValue<string>("Dir");
            if (string.IsNullOrWhiteSpace(logPath))
                logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

            var allfile = Path.Combine(logPath, $"nlog-all-{DateTime.Now:yyyy-MM-dd}.log");
            var ownFile = Path.Combine(logPath, $"nlog-own-{DateTime.Now:yyyy-MM-dd}.log");
            var internalLog = Path.Combine(logPath, $"internal-nlog.txt");


            if (File.Exists(allfile))
                result.Add("nlog-all", await File.ReadAllTextAsync(allfile));
            if (File.Exists(ownFile))
                result.Add("nlog-own", await File.ReadAllTextAsync(ownFile));
            if (File.Exists(internalLog))
                result.Add("internal-nlog", await File.ReadAllTextAsync(internalLog));
            return result;
        }
    }
}
