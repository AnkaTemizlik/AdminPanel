using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Logging;
using PointmentApp.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PointmentApp.Services {
    
    [Service(typeof(IScopedProcessingService), Lifetime.Scoped)]
    public class AppProcessJob : IScopedProcessingService {
        public string ProcessName => nameof(AppProcessJob);

        private readonly ILogger<AppProcessJob> _logger;

        public AppProcessJob(ILogger<AppProcessJob> logger) {
            _logger = logger;
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
