using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DNA.API.Models {

    [Table("{TablePrefix}USER")]
    public class User {
        public string FullName { get; set; }

        [JsonIgnore]
        public string Role { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public string EmailConfirmationCode { get; set; }

        [JsonIgnore]
        public string PasswordConfirmationCode { get; set; }

        public bool IsInitialPassword { get; set; }
    }

    public class ApplicationUser : User {
        public int Id { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string Token { get; set; }

        public int ExpiresIn { get; set; } = 1 * 60 * 60 * 12;

        List<string> _Roles;
        public List<string> Roles {
            get {
                if (_Roles == null)
                    _Roles = string.IsNullOrWhiteSpace(Role)
                        ? new List<string> { "Reader" }
                        : Role.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToList();
                return _Roles;
            }
        }

        [JsonIgnore]
        public string PictureUrl { get; set; }

        [JsonIgnore]
        public string Location { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEnd { get; set; }
        

    }
}
