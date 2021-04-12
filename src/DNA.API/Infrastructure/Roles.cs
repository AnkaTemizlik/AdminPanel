using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Infrastructure {
    public static class Roles {
        public const string User = "User";
        public const string Reader = "Reader";
        public const string Writer = "Writer";
        public const string Admin = "Admin";
        public const string NoX = "NoX";
    }

    public static class Policies {
        public const string ReadOnly = "ReadOnlyPolicy";
        public const string WriteOnly = "WriteOnlyPolicy";
        //public const string EditOnly = "EditOnlyPolicy";
        //public const string WriteOrEdit = "WriteOrEditPolicy";
        public const string AdminOnly = "AdminOnlyPolicy";
    }
}
