namespace TextAdventure.Security;

public sealed class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class LoginResponse
{
    public string Token { get; set; } = "";
    public LoginUser User { get; set; } = new();
}

public sealed class LoginUser
{
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
}

public sealed class KeyshareResponse
{
    public string RoomId { get; set; } = "";
    public string Keyshare { get; set; } = "";
}
