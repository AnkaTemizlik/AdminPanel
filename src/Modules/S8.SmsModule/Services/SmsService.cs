using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using S8.SmsModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S8.SmsModule.Services {
    public interface ISmsService {
        Task<int> InsertAsync(string to, string text, DateTime? scheduledPandingTime, int flag);
        Task<bool> UpdateAsync(int smsId, string to, string text, DateTime? scheduledPandingTime);
    }

    [Service(typeof(ISmsService), Lifetime.Scoped)]
    public class SmsService : ISmsService {

        private readonly ILogger<SmsService> _logger;
        private readonly IProcessService _processService;
        private readonly IConfiguration _configuration;

        public SmsService(ILogger<SmsService> logger, IProcessService processService, IConfiguration configuration) {
            _logger = logger;
            _processService = processService;
            _configuration = configuration;
        }

        public async Task<int> InsertAsync(string to, string text, DateTime? scheduledPandingTime, int flag) {
            var id = await _processService.InsertAsync(new Sms {
                Message = text,
                PhoneNumber = to,
                ScheduledSendingTime = scheduledPandingTime,
                Flags = flag
            });
            return id;
        }

        public async Task<bool> UpdateAsync(int smsId, string to, string text, DateTime? scheduledPandingTime) {
            var ok = await _processService.UpdateAsync(new Sms {
                Id = smsId,
                Message = text,
                PhoneNumber = to,
                ScheduledSendingTime = scheduledPandingTime
            });
            return ok;
        }
    }
}
