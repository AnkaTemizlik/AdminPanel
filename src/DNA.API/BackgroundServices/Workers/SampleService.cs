using DNA.Domain.Exceptions;
using DNA.Domain.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DNA.API.BackgroundServices.Workers {
    public class SampleService : IScopedProcessingService {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobs;
        private readonly ILogger<SampleService> _logger;
        public SampleService([NotNull] IBackgroundJobClient backgroundJobs,
            [NotNull] IRecurringJobManager recurringJobs,
            [NotNull] ILogger<SampleService> logger) {

            _backgroundJobs = backgroundJobs ?? throw new ArgumentNullException(nameof(backgroundJobs));
            _recurringJobs = recurringJobs ?? throw new ArgumentNullException(nameof(recurringJobs));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string ProcessName => "SampleService";

        public Task DoWork(CancellationToken stoppingToken) {
            try {
                //_backgroundJobs.Enqueue<>(x => x.LongRunning(JobCancellationToken.Null));

                _recurringJobs.AddOrUpdate("SAMPLE for seconds", () => Console.WriteLine("Hello, seconds!"), "*/15 * * * * *");
                _recurringJobs.AddOrUpdate("SAMPLE for minutely", () => Console.WriteLine("Hello, world!"), Cron.Minutely);
                _recurringJobs.AddOrUpdate("SAMPLE for hourly", () => Console.WriteLine("Hello"), "25 15 * * *");
                _recurringJobs.AddOrUpdate("SAMPLE for neverfires", () => Console.WriteLine("Can only be triggered"), "0 0 31 2 *");
                _recurringJobs.AddOrUpdate("SAMPLE for Hawaiian", () => Console.WriteLine("Hawaiian"), "15 08 * * *", TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time"));
                _recurringJobs.AddOrUpdate("SAMPLE for UTC", () => Console.WriteLine("UTC"), "15 18 * * *");
                _recurringJobs.AddOrUpdate("SAMPLE for Russian", () => Console.WriteLine("Russian"), "15 21 * * *", TimeZoneInfo.Local);
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.HostedServiceStoped, ex, ("ProcessName", ProcessName));
            }

            return Task.CompletedTask;
        }

        public Task Init() {
            throw new NotImplementedException();
        }

        //public void Init(INoXService noXService, ILogger logger, IConfiguration configuration, IProcessService processService) {

        //}
    }
}
