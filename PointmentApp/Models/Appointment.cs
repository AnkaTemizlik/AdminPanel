using Dapper.Contrib.Extensions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}Appointment")]
    [JsonRestricted]
    public class Appointment : Model, IChangeTrackable {

        [Column]
        [StringLength(500)]
        [Required]
        public string Title { get; set; }

        [Column(LookupType = typeof(Customer))]
        [Required]
        public int CustomerId { get; set; }

        [Column(LookupType = typeof(Service))]
        public int? ServiceId { get; set; }

        [Column] public bool AllDay { get; set; }

        [Column] public DateTime? StartDate { get; set; }
        [Column] public DateTime? EndDate { get; set; }
        [Column] [Required] public AppointmentState State { get; set; }
        [Column] [Required] public PriorityType Priority { get; set; } = PriorityType.Normal;
        [Column] [StringLength(500)] public string RecurrenceRule { get; set; }
        [Column] [StringLength(500)] public string RecurrenceException { get; set; }
        [Column] [StringLength(int.MaxValue)] public string Note { get; set; }
        
        [Column(Currency = "TRY")] 
        [JsonRestrictedRole("Admin", "Writer")]
        public double? Amount { get; set; }

        // for Exploration
        [Column] public bool IsPlanned { get; set; }
        [Column] public int CreatedBy { get; set; }
        [Column] public int UpdatedBy { get; set; }
        [JsonIgnore] [Computed] public List<Document> Documents { get; set; }
        [Column(LookupType = typeof(DNA.API.Models.User))] public int? AssignTo { get; set; }
        [JsonIgnore] [Computed] public List<S8.SmsModule.Models.Sms> SortMessages { get; set; }

        //[Computed] public string PhoneNumber { get; set; }
        //[Computed] public string CustomerName { get; set; }
        //[Computed] public string ServiceName { get; set; }
    }
}
