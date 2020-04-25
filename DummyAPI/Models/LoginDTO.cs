namespace DummyAPI.Models
{
    public class LoginDTO
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public bool RememberMe { get; set; }
    }
}
