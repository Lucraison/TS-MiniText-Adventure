namespace TextAdventure.Api.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Player"; // optioneel, default Player
    }
}
