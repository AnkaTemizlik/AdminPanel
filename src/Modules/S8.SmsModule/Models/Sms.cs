using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace S8.SmsModule.Models {

    [Table("{TablePrefix}Sms")]
    public class Sms : Model {
        [Column] [Required] [StringLength(50)] public string PhoneNumber { get; set; }
        [Column] [Required] [StringLength(155)] public string Message { get; set; }
        [Column] public DateTime? SendTime { get; set; }
        [Column] [StringLength(50)] public string Sender { get; set; }

        /// <summary>
        /// turkce, normal
        /// </summary>
        [Column] public bool? InTurkish { get; set; }
        [Column] public DateTime? ScheduledSendingTime { get; set; }
        [Column] public bool Sent { get; set; }
        [Column] [StringLength(1000)] public string Response { get; set; }
        [Column] public int Flags { get; set; }
    }
}
