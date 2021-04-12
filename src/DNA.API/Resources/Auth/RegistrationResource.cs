
namespace DNA.API.Services.Communication {
    public class RegistrationResource {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Role { get; set; }

        //public string Recaptcha { get; set; }
        public string Key { get; set; }

    }
}
