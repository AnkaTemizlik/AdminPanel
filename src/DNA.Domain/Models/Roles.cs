using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.Domain.Models {
    public static class Roles {
        public const string User = "User";
        public const string Reader = "Reader";
        public const string Writer = "Writer";
        public const string Admin = "Admin";

        public static List<IdValue<string>> Values() {
            return new List<IdValue<string>> {
            new IdValue<string>("Admin","Admin"),
            new IdValue<string>("Writer","Writer"),
            new IdValue<string>("Reader","Reader"),
            new IdValue<string>("User","User")
            };
        }
    }

    public static class Policies {
        public const string ReadOnly = "ReadOnlyPolicy";
        public const string WriteOnly = "WriteOnlyPolicy";
        //public const string EditOnly = "EditOnlyPolicy";
        //public const string WriteOrEdit = "WriteOrEditPolicy";
        public const string AdminOnly = "AdminOnlyPolicy";
    }
}
