using PointmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Resources {
    public class AppointmentResource {
        public string Title { get; set; }
        public int CustomerId { get; set; }
        public int? ServiceId { get; set; }
        public bool AllDay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AppointmentState State { get; set; }
        public PriorityType Priority { get; set; }
        public string Note { get; set; }
        public bool IsPlanned { get; set; }
        public string RecurrenceRule { get; set; }
    }
}
