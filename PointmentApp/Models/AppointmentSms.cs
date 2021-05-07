using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}AppointmentSms")]
    public class AppointmentSms : Model {
        [Column] [Required] public int AppointmentId { get; set; }
        [Column] [Required] public int SmsId { get; set; }

        //[Computed] public DateTime? ScheduledSendingTime { get; set; }
        //[Computed] public bool Sent { get; set; }
        //[Computed] [StringLength(1000)] public string LastStatus { get; set; }
    }
}
