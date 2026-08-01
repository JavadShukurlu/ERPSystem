namespace ERPSystem.WebMVC.ViewModels.Auth
{
    public class AuthResponseViewModel
    {
        public string Token { get; set; } = null!;

        public DateTime Expiration { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public IList<string> Roles { get; set; } = new List<string>();
    }
}