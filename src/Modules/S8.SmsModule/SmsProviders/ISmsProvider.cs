using S8.SmsModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S8.SmsModule.SmsProviders {
    public interface ISmsProvider {
        Task<string> SendToANumberAsync(Sms sms);
    }
}
