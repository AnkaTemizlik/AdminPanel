using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Models {
    public class Log {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string EntityName { get; set; }
        public string EntityKey { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }

    public enum LogLevelTypes {
        Info,
        Warn,
        Error,
        Debug
    }
}
