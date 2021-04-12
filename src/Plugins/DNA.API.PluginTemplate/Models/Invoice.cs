using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Models {

    [Table("{TablePrefix}INVOICE")]
    public class Invoice : Model {
        [ExplicitKey] [Column] public string InvoiceCode { get; set; }
        [Column] public InvoiceState State { get; set; }
        [Column] public string LastError { get; set; }

        internal void SetState(InvoiceState state) {
            State = state;
            UpdateTime = DateTime.Now;
            LastError = null;
        }

        internal void SetState(string errorMessage) {
            LastError = errorMessage;
            UpdateTime = DateTime.Now;
        }

        internal void SetState(Invoice o) {
            LastError = null;
            Id = o.Id;
            State = o.State;
            CreationTime = o.CreationTime;
            //DocumentNumber = o.DocumentNumber;
            //LegalNumber = o.LegalNumber;
            //InvoiceStatus = o.InvoiceStatus;
            //InvoiceType = o.InvoiceType;
            //ETTN = o.ETTN;
            //EnvelopeStatus = o.EnvelopeStatus;
        }
    }
}
