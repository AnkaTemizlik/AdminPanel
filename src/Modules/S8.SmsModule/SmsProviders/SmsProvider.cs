using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S8.SmsModule.SmsProviders {
    public class SmsProviderFactory {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smsConfiguration">
        /// <para>Must be root IConfiguration</para>
        /// </param>
        public ISmsProvider Create(IConfiguration configuration) {
            var smsSettingsSection = configuration.GetSection("Config:SmsSettings");
            var activeProviderName = smsSettingsSection.GetValue<string>("ActiveProviderName");
            var settings = smsSettingsSection.GetSection(activeProviderName);
            return activeProviderName switch {
                "VatanSMS" => new VatanSMS.VatanSmsProvider(settings),
                _ => throw new NotImplementedException("SmsSettings:ActiveProviderName is not implemented."),
            };
        }
    }
}
