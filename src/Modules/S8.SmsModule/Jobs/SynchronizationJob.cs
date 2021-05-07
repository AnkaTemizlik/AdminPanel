using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using S8.SmsModule.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S8.SmsModule.Jobs {
    [Service(typeof(IScopedProcessingService), Lifetime.Scoped)]
    public class SynchronizationJob : IScopedProcessingService {
        public string ProcessName => nameof(SynchronizationJob);

        private readonly IConfiguration _configuration;
        private readonly ILogger<SynchronizationJob> _logger;
        private readonly IProcessService _processService;

        public SynchronizationJob(IConfiguration configuration, ILogger<SynchronizationJob> logger, IProcessService processService) {
            _logger = logger;
            _configuration = configuration;
            _processService = processService;
        }

        public async Task DoWork(CancellationToken stoppingToken) {
            try {
                _logger.LogInformation(Alerts.JobCompleted, ("ProcessName", ProcessName));
            }
            catch (Exception exRoot) {
                _logger.LogError(AlertCodes.JobError, exRoot, ("ProcessName", ProcessName));
            }

            await Task.CompletedTask;
        }

        public async Task Init() {
            _logger.LogInformation(Alerts.JobInit, (ProcessName, ""));
            await Task.CompletedTask;
        }
    }
}
