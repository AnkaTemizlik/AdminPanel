using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DNA.Domain {
    public static class Helpers {
        public static TaskAwaiter<T> GetAwaiter<T>(this Lazy<Task<T>> asyncTask) {
            return asyncTask.Value.GetAwaiter();
        }
    }
}
