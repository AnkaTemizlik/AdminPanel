using DNA.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Resources.Auth {
    public class UserQueryResource : QueryResource {
        public int UserId { get; set; }
    }
}
