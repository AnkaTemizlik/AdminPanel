using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Resources.Auth {
    public class ApplicationUserResource {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsInitialPassword { get; set; }
        public string PasswordConfirmationCode { get; set; }
    }
}
