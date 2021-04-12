using DNA.API.PluginTemplate.Exceptions;
using DNA.API.PluginTemplate.Models;
using DNA.Domain.Exceptions;
using DNA.Domain.Visitors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Services {
    public class AddInvoiceVisitor : Visitable<IInvoiceService, InvoiceState> {

        readonly Invoice _invoice;

        public AddInvoiceVisitor(Invoice invoice, InvoiceState outputState) : base(outputState) {
            _invoice = invoice;
        }
         
        public async override Task DoAsync() {
            var manager = Manager as InvoiceVisitManager;

            try {
                //await service.AddInvoiceAsync(_invoice);
                //Manager.Valuer.SetCurrentModel(service.CurrentInvoice, service.CurrentInvoice.Customer);
                //await Manager.ProcessService.UpdateAsync(service.CurrentInvoice);
                //Manager.Logger.LogInformation(NetsisAlerts.AddDocumentInfo, ("InvoiceCode", _invoice.InvoiceCode));
                await Task.CompletedTask;
            }
            catch (Exception ex) {
                //var alert = Manager.Logger.LogError(NetsisAlerts.AddDocumentError, ex);
                //if (service.CurrentInvoice != null) {
                //    service.CurrentInvoice.SetState($"{alert.Message}");
                //    await Manager.ProcessService.UpdateAsync(service.CurrentInvoice);
                //}
                //Cancel(alert);
            }
        }
    }
    public class CheckStockVisitor : Visitable<IInvoiceService, InvoiceState> {
        public CheckStockVisitor(InvoiceState inputState, InvoiceState outputState) : base(inputState, outputState) { }
        public async override Task DoAsync() {
            var manager = Manager as InvoiceVisitManager;
            await Task.CompletedTask;
        }
    }
    public class CheckCustomerVisitor : Visitable<IInvoiceService, InvoiceState> {
        public CheckCustomerVisitor(InvoiceState inputState, InvoiceState outputState) : base(inputState, outputState) { }

        public async override Task DoAsync() {
            var manager = Manager as InvoiceVisitManager;
            await Task.CompletedTask;
        }
    }

    public class SendInvoiceVisitor : Visitable<IInvoiceService, InvoiceState> {
        public SendInvoiceVisitor(InvoiceState inputState, InvoiceState outputState) : base(inputState, outputState) { }

        public async override Task DoAsync() {
            var manager = Manager as InvoiceVisitManager;
            await Task.CompletedTask;
        }
    }

    public class TriggerEAdaptorVisitor : Visitable<IInvoiceService, InvoiceState> {
        public TriggerEAdaptorVisitor(InvoiceState inputState, InvoiceState outputState) : base(inputState, outputState) { }

        public async override Task DoAsync() {
            var manager = Manager as InvoiceVisitManager;
            await Task.CompletedTask;
        }
    }

    public class CompleterVisitor : Visitable<IInvoiceService, InvoiceState> {
        public CompleterVisitor(InvoiceState inputState, InvoiceState outputState) : base(inputState, outputState) { }

        public async override Task DoAsync() {
            var manager = Manager as InvoiceVisitManager;
            await Task.CompletedTask;
        }
    }
}
