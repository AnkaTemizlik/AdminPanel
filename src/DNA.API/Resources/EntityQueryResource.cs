using DNA.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Resources {
    public class EntityQueryResource : QueryResource {
        public string Name { get; set; }
    }
}
