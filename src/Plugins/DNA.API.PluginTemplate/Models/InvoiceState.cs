using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Models {
    public enum InvoiceState {
        None = 0,
        Saved = 1,
        SendStockToNoX = 2,
        SendCurrentAccountToNoX = 3,
        SendDocumentToNoX = 4,
        TriggerEAdaptor = 6,
        // InquiryTaxpayer = 9,
        Completed = 10
    }
}
