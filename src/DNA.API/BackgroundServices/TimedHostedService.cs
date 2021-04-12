using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNA.Domain.Exceptions;
using DNA.Domain.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DNA.API.BackgroundServices {
    public class TimedHostedService : BackgroundService {
        private readonly IRecurringJobManager _recurringJobs;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IConfiguration _configuration;
        public IServiceProvider _Services { get; }
        //readonly IServiceScopeFactory _serviceScopeFactory;

        public TimedHostedService(IServiceProvider services,
            //IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            IRecurringJobManager recurringJobs,
            IBackgroundJobClient backgroundJobs,
            IWritableOptions configEditor,
            ILogger<TimedHostedService> logger) {

            _Services = services;
            //_serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _recurringJobs = recurringJobs;
            _backgroundJobs = backgroundJobs;
            _logger = logger;

#if DEBUG
            _logger.LogInformation($"ctor {nameof(TimedHostedService)}");
#endif
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken) {

            try {

                using var scope = _Services.CreateScope();

                var workerEnabled = _configuration.GetSection("Worker").GetValue<bool>("Enabled");

                if (workerEnabled) {

                    // Startup Manager
                    foreach (var startupManager in scope.ServiceProvider.GetServices<IPluginStartupManager>()) {
                        _backgroundJobs.Enqueue(() => startupManager.DoWork());
                        //_backgroundJobs.Schedule(() => startupManager.DoWork(), TimeSpan.FromSeconds(2));
                        _logger.LogInformation(AlertCodes.HostedServiceRunning, ("StartupManager", $"{startupManager.Name}"));
                    }

                    // Recurring Jobs
                    foreach (var processingService in scope.ServiceProvider.GetServices<IScopedProcessingService>()) {

                        var jobSection = _configuration.GetSection($"Config:RecurringJobs:{processingService.ProcessName}");
                        var jobEnabled = jobSection.GetValue<bool>("Enabled");
                        if (!jobEnabled) {
                            _recurringJobs.RemoveIfExists(processingService.ProcessName);
                            continue;
                        }
                        
                        await processingService.Init();

                        string cron = jobSection.GetValue<string>("Cron") ?? _configuration[$"Worker:Cron"] ?? "0 * * * *";
                        _recurringJobs.AddOrUpdate(
                            processingService.ProcessName,
                            () => processingService.DoWork(stoppingToken),
                            cron,
                            TimeZoneInfo.Local
                            );
                        _logger.LogInformation(AlertCodes.HostedServiceStarting, ("ProcessingService", processingService.ProcessName), ("Cron", cron));

                    }
                }

            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.HostedServiceStoped, ex, ("BackgroundService", "TimedHostedService"));
            }
        }

    }
}
