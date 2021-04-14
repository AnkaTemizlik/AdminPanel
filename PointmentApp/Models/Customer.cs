using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}Customer")]
    public class Customer : Model {
        [Column] [Required] [StringLength(250)] public string Name { get; set; }
        [Column] [Required] [StringLength(16)] public string PhoneNumber { get; set; }
        [Column] [Required] public bool Corporate { get; set; }
        [Column] [StringLength(500)] public string Title { get; set; }
        [Column] [Required] [StringLength(50)] public string Email { get; set; }
        [Column] [StringLength(16)] public string LandlinePhoneNumber { get; set; }
        [Column] [StringLength(16)] public string MobilePhoneNumber { get; set; }
        [Column] [StringLength(500)] public string Address { get; set; }
        [Column] [Required] public int CityId { get; set; }
        [Column] [StringLength(11)] public string TaxNumber { get; set; }
        [Column] [StringLength(50)] public string TaxAdministration { get; set; }
        [Column] [StringLength(500)] public string BillingAddress { get; set; }
        [Column] [StringLength(int.MaxValue)] public string Note { get; set; }

    }
}
