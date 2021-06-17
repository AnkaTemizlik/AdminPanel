using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {
    public enum AppointmentState {
        None = 0,
        Assigned = 2,
        //Deleted = 8,
        Canceled = 9,
        Completed = 10
    }
}
