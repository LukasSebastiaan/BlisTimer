namespace BlisTimer.Models
{
    public class LoginForm
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
        public string? Status { get; set; }
    }
}
