namespace ClubScansub.BlazorApp.Areas.Identity
{
    public class LoginUser<TUser> where TUser : class
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public TUser User { get; set; }

        public string TwoFactorCode { get; set; }

        public bool RememberMe { get; set; }

        public bool RememberMachine { get; set; }

        public string ReturnUrl { get; set; } = "/";

        public DateTime LoginStarted { get; set; }

        public string Error { get; set; }
    }
}
