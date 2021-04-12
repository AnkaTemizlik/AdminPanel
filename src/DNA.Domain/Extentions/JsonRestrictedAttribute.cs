using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DNA.Domain.Extentions {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JsonRestrictedAttribute : Attribute {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class JsonRestrictedRoleAttribute : Attribute {

        public List<string> Roles { get; set; }

        public JsonRestrictedRoleAttribute(params string[] roles) {
            Roles = roles.ToList();
        }

        public bool IsRestricted(System.Security.Principal.IPrincipal user) {
            var ok = Roles.Count > 0 && Roles.All(_ => user.IsInRole(_));
            return ok;
        }
    }
}
