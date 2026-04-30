namespace FrogLib.Server.DTOs
{
    public class RegisterUser
    {
        public string NameUser { get; set; }

        public string LoginUser { get; set; }

        public string PasswordHash { get; set; }
        public string ConfirmPassword { get; set; }

        public IFormFile? ProfileImageUrl { get; set; }
    }
}
