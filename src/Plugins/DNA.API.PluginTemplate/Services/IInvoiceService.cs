using DNA.API.PluginTemplate.Models;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Services {
    public interface IInvoiceService {
        Invoice CurrentInvoice { get; set; }

        Task DoWork();
    }
}
