using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Resources {
    public class LogResource {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public string Logged { get; set; }
        public string Level { get; set; }
        public string EntityName { get; set; }
        public string EntityKey { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }
}
