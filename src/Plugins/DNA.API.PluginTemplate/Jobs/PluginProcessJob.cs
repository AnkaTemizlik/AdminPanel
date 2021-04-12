using DNA.API.PluginTemplate.Exceptions;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Services {
    [Service(typeof(IScopedProcessingService), Lifetime.Scoped)]
    public class PluginProcessJob : IScopedProcessingService {
        public string ProcessName => nameof(PluginProcessJob);

        private readonly ILogger<PluginProcessJob> _logger;
        private readonly IInvoiceService _invoiceService;

        public PluginProcessJob(ILogger<PluginProcessJob> logger, IInvoiceService invoiceService) {
            _logger = logger;
            _invoiceService = invoiceService;
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
                await _invoiceService.DoWork();
            }
            catch (Exception ex) {
                _logger.LogError(PluginAlertCodes.DoWorkException, ex, ("ProcessName", ProcessName));
            }
        }
    }
}
