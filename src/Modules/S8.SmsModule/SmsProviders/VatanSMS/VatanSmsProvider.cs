using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using S8.SmsModule;
using S8.SmsModule.Models;

namespace S8.SmsModule.SmsProviders.VatanSMS {
    public class VatanSmsProvider : ISmsProvider {

        OztekSmsWebService.OztekSmsWebServicePortTypeClient _client;
        readonly Options _options = new Options();

        /// <summary>
        /// https://pathfindertech.net/connecting-to-a-php-web-service-with-wcf-in-c/
        /// https://stackoverflow.com/questions/7033442/using-iso-8859-1-encoding-between-wcf-and-oracle-linux
        /// https://docs.microsoft.com/en-us/dotnet/framework/wcf/samples/custom-message-encoder-custom-text-encoder?redirectedfrom=MSDN
        /// </summary>
        public VatanSmsProvider(IConfigurationSection section) {
            section.Bind(_options);
            //CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11), new HttpsTransportBindingElement()); // Or  HttpsTransportBindingElement

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            EndpointAddress endpointAddress = new EndpointAddress(_options.RemoteAddress);
            _client = new OztekSmsWebService.OztekSmsWebServicePortTypeClient(binding, endpointAddress);

            //_client.Endpoint.Binding = binding;
            //_client.Endpoint.Address = endpointAddress;
        }

        public async Task<string> SendToANumberAsync(Sms sms) {
            try {
                sms.Sender ??= _options.Originator;
                var result = await _client.TekSmsiBirdenCokNumarayaGonderAsync(
                     kullanicino: _options.CustomerCode,
                     kullaniciadi: _options.UserName,
                     sifre: _options.Password,
                     orjinator: sms.Sender ,
                     numaralar: sms.PhoneNumber,
                     mesaj: sms.Message,
                     zaman: sms.SendTime == null ? "" : sms.SendTime.Value.ToString(_options.SendTimeFormat),
                     zamanasimi: "",
                     tip: sms.InTurkish.GetValueOrDefault(true) ? "turkce" : "normal"
                     );
                return result.@return;

            }
            catch (Exception ex) {
                var match = Regex.Match(ex.Message, "<return xsi:type =\"xsd:string\">(.*)</return>");
                var message = ex.Message;
                if (match.Groups != null && match.Groups.Count > 0 && !string.IsNullOrWhiteSpace(match.Groups[0].Value))
                    message = match.Groups[0].Value;
                throw new Exception(message);
            }
        }
    }
}
