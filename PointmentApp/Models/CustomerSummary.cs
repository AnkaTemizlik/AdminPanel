using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DapperKey = Dapper.Contrib.Extensions.KeyAttribute;

namespace PointmentApp.Models {

    [Table("{TablePrefix}" + nameof(CustomerSummary))]
    public class CustomerSummary {
        [DapperKey] public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int AppointmentCount { get; set; }
        public int CityId { get; set; }
        public DateTime LastAppointmentDate { get; set; }
    }
}
