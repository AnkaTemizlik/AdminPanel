using DNA.Domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Configuration;
using DNA.Domain.Exceptions;

namespace DNA.API.BackgroundServices {
    public class ConsumeScopedServiceHostedService : BackgroundService {
        private readonly ILogger<ConsumeScopedServiceHostedService> _logger;
        private readonly IRecurringJobManager _recurringJobs;
        private readonly IConfiguration _configuration;

        public ConsumeScopedServiceHostedService(IServiceProvider services,
            ILogger<ConsumeScopedServiceHostedService> logger,
            IConfiguration configuration,
            IRecurringJobManager recurringJobs) {
            _configuration = configuration.GetSection("Worker");
            _recurringJobs = recurringJobs;
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation(AlertCodes.HostedServiceStarting);
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            try {
                string cron = _configuration["Cron"] ?? "0 * * * *";
                _logger.LogInformation(AlertCodes.HostedServiceRunning, ("Cron", cron));
                using (var scope = Services.CreateScope()) {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();
                    _recurringJobs.RemoveIfExists(scopedProcessingService.ProcessName);
                    _recurringJobs.AddOrUpdate(scopedProcessingService.ProcessName, () => scopedProcessingService.DoWork(stoppingToken), cron);
                    await scopedProcessingService.DoWork(stoppingToken);
                }
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.HostedServiceStoped, ex);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation(AlertCodes.HostedServiceStoping);
            await Task.CompletedTask;
        }
    }
}
