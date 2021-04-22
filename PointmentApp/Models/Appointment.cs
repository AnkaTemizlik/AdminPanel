using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}Appointment")]
    public class Appointment : Model, IChangeTrackable {

        [Column] 
        [StringLength(500)] 
        [Required] 
        public string Title { get; set; }

        [Column(LookupType = typeof(Customer))] 
        [Required] 
        public int CustomerId { get; set; }

        [Column(LookupType = typeof(Service))] 
        [Required] 
        public int ServiceId { get; set; }

        [Column] public bool AllDay { get; set; }

        [Column] [Required] public DateTime StartDate { get; set; }
        [Column] public DateTime? EndDate { get; set; }
        [Column] [Required] public AppointmentState State { get; set; }
        [Column] [Required] public PriorityType Priority { get; set; }
        [Column] [StringLength(int.MaxValue)] public string Note { get; set; }
        [Computed] public int CreatedBy { get; set; }
        [Computed] public int UpdatedBy { get; set; }
        [JsonIgnore] [Computed] public List<Document> Documents { get; set; }
        [JsonIgnore] [Computed] public List<AppointmentEmployee> AssignTo { get; set; }
    }
}
