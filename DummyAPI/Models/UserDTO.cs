namespace DummyAPI.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public bool Active { get; set; }
        public string Password { get; set; } //Este password NUNCA se guarda, puesto que está en texto plano
        public string PasswordHash { get; set; } //Este es el password seguro que podría almacenarse en una base de datos
    }
}
