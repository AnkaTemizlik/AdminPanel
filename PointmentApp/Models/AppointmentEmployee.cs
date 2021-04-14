using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    /// <summary>
    /// Writer
    /// </summary>
    [Table("{TablePrefix}" + nameof(AppointmentEmployee))]
    public class AppointmentEmployee : Model {
        [Column] [Required] public int AppointmentId { get; set; }
        [Column] [Required] public int UserId { get; set; }
    }
}
