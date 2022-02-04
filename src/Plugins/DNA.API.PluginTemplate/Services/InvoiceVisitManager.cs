using DNA.API.PluginTemplate.Models;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using DNA.Domain.Visitors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Services {
    [Service(typeof(IScopedVisitManager), Lifetime.Scoped)]
    public class InvoiceVisitManager : VisitManager<IInvoiceService, InvoiceState>, IScopedVisitManager {
        public string VisitorName { get; set; } = nameof(InvoiceVisitManager);

        //public INoXService NoX { get; }

        public event EventHandler<Response> ResponseHandler;

        public InvoiceVisitManager(IConfiguration configuration, IInvoiceService invoiceService, ILogger<VisitManager<IInvoiceService, InvoiceState>> logger, IProcessService processService, IEmailService emailService, IValuerService valuerService)
            : base(configuration, logger, processService, emailService, valuerService) {
            DataStore = invoiceService;
            //NoX = noXService;
        }

        public async Task DoWork<T>(T data) {
            try {
                var invoice = data as Invoice;

                var pool = new VisitorPool<IInvoiceService, InvoiceState>();
                //pool.OnCanceled += (s, e) => {
                //    ResponseHandler?.Invoke(this, new Response(e));
                //};
                //pool.Add(new AddInvoiceVisitor(invoice, InvoiceState.Saved));
                //pool.Add(new CheckStockVisitor(InvoiceState.Saved, InvoiceState.SendStockToNoX));
                //pool.Add(new CheckCustomerVisitor(InvoiceState.SendStockToNoX, InvoiceState.SendCurrentAccountToNoX));
                //pool.Add(new SendInvoiceVisitor(InvoiceState.SendCurrentAccountToNoX, InvoiceState.SendDocumentToNoX));
                //pool.Add(new TriggerEAdaptorVisitor(InvoiceState.SendDocumentToNoX, InvoiceState.TriggerEAdaptor));
                //pool.Add(new CompleterVisitor(InvoiceState.TriggerEAdaptor, InvoiceState.Completed));

                var canceled = await pool.Accept(this);
                if (!canceled)
                    ResponseHandler?.Invoke(this, new Response(invoice));

                Logger.LogInformation(AlertCodes.JobCompleted, ("VisitorName", VisitorName));
            }
            catch (Exception ex) {
                var alert = Logger.LogError(AlertCodes.JobStoped, ex, ("VisitorName", VisitorName));
                ResponseHandler?.Invoke(this, new Response(alert));
            }
        }

        public override InvoiceState GetModelState() {
            if (DataStore.CurrentInvoice == null)
                return InvoiceState.None;
            return DataStore.CurrentInvoice.State;
        }
    }
}
