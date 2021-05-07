using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using S8.SmsModule.Exceptions;
using S8.SmsModule.Models;
using S8.SmsModule.SmsProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S8.SmsModule.Jobs {
    [Service(typeof(IScopedProcessingService), Lifetime.Scoped)]
    public class SmsSyncJob : IScopedProcessingService {
        public string ProcessName => nameof(SmsSyncJob);

        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsSyncJob> _logger;
        private readonly IProcessService _processService;

        public SmsSyncJob(IConfiguration configuration, ILogger<SmsSyncJob> logger, IProcessService processService) {
            _logger = logger;
            _configuration = configuration;
            _processService = processService;
        }

        public async Task DoWork(CancellationToken stoppingToken) {
            try {
                var smsProviderFactory = new SmsProviderFactory();
                var smsProvider = smsProviderFactory.Create(_configuration);

                var messages = await _processService.QueryAsync<Sms>(SqlQueries.SelectPendingShortMessages);

                foreach (Sms sms in messages) {
                    try {
                        var result = await smsProvider.SendToANumberAsync(sms);
                        sms.Response = result;
                    }
                    catch (Exception ex) {
                        sms.Response = ex.Message;
                    }
                    sms.Sent = true;
                    sms.UpdateTime = DateTime.Now;
                    await _processService.UpdateAsync(sms);
                }
                _logger.LogInformation(Alerts.JobCompleted, ("ProcessName", ProcessName));
            }
            catch (Exception exRoot) {
                _logger.LogError(AlertCodes.JobError, exRoot, ("ProcessName", ProcessName));
            }
        }

        public async Task Init() {
            try {
                await _processService.ExecuteAsync(SqlQueries.Migration);
                _logger.LogInformation(Alerts.JobInit, ("ProcessName", ProcessName), ("Migration", ""));
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.JobError, ex, ("ProcessName", ProcessName));
            }
        }
    }
}
