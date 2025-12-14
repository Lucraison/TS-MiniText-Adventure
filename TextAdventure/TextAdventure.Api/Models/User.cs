namespace TextAdventure.Api.Models
{
    public class User
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Player"; // "Player" of "Admin"

        public int FailedLoginCount { get; set; } = 0;
        public bool IsLockedOut { get; set; } = false;
    }
}
