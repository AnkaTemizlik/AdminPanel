using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DNA.API.Models {

    [Table("{TablePrefix}USER")]
    public class User : Model {

        [Column] public bool IsDeleted { get; set; }
        [Column(DisplayExpr = true)] [Required] [StringLength(150)] public string FullName { get; set; }
        [Column] [Required] [StringLength(50)] public string Role { get; set; }
        [Column] [Required] [StringLength(100)] public string Email { get; set; }
        [Column] [StringLength(50)] public string PhoneNumber { get; set; }
        [Column] public bool EmailConfirmed { get; set; }
        [Column] public bool IsInitialPassword { get; set; }
        [Column] [JsonIgnore] public string EmailConfirmationCode { get; set; }
        [Column] [JsonIgnore] public string PasswordConfirmationCode { get; set; }
        [Column(Image = true)] [StringLength(int.MaxValue)] public string PictureUrl { get; set; }
        [Column] [StringLength(500)] public string MainModules { get; set; }
    }

    public class ApplicationUser : User {

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
        public string Location { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEnd { get; set; }


    }
}
