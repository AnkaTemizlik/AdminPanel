using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services.Communication {
    public class ChangePasswordResource {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        //public string Recaptcha { get; set; }
        public string Code { get; set; }
    }
}
