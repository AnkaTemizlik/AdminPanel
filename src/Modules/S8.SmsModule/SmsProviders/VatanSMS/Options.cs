using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S8.SmsModule.SmsProviders.VatanSMS {
    public class Options {
        public string RemoteAddress { get; set; }
        public string CustomerCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Originator { get; set; }
        public string CancelationLink { get; set; }
        public string SendTimeFormat { get; set; }
    }
}
