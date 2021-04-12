using DNA.API.PluginTemplate.Exceptions;
using DNA.API.PluginTemplate.Models;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Services {

    [Service(typeof(IInvoiceService), Lifetime.Scoped)]
    public class InvoiceService : IInvoiceService {

        private readonly IConfiguration _configuration;
        public Invoice CurrentInvoice { get; set; }

        public InvoiceService(IConfiguration configuration) {
            _configuration = configuration;
        }

        public async Task DoWork() {
            await Task.CompletedTask;
        }
    }
}
