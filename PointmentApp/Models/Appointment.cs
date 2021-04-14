using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}Appointment")]
    public class Appointment : Model {
        [Column] [Required] public int CustomerId { get; set; }
        [Column] [Required] public int ServiceId { get; set; }
        [Column] [Required] public DateTime StartDate { get; set; }
        [Column] [Required] public DateTime EndDate { get; set; }
        [Column] [Required] public AppointmentState State { get; set; }
        [Column] [Required] public PriorityType Priority { get; set; }
        [Column] [StringLength(int.MaxValue)] public string Note { get; set; }
        [Column] public int CreatedBy { get; set; }
        [Computed] public List<Document> Documents { get; set; }
        [Computed] public List<AppointmentEmployee> AssignTo { get; set; }
    }
}
