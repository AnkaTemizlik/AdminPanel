using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Logging;
using PointmentApp.Exceptions;
using S8.SmsModule.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PointmentApp.Services {
    
    [Service(typeof(IScopedProcessingService), Lifetime.Scoped)]
    public class SyncJob : IScopedProcessingService {
        public string ProcessName => nameof(SyncJob);

        private readonly ILogger<SyncJob> _logger;
        private readonly ISmsService _smsService;

        public SyncJob(ILogger<SyncJob> logger, ISmsService smsService) {
            _logger = logger;
            _smsService = smsService;
        }

        public async Task Init() {
            try {
                _logger.LogInformation(AlertCodes.JobInit);
                await Task.CompletedTask;
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.JobError, ex);
            }
        }

        public async Task DoWork(CancellationToken stoppingToken) {
            try {
                _logger.LogInformation(PluginAlertCodes.DoWorkInfo);
                await Task.CompletedTask;
            }
            catch (Exception ex) {
                _logger.LogError(PluginAlertCodes.DoWorkException, ex, ("ProcessName", ProcessName));
            }
        }
    }
}
