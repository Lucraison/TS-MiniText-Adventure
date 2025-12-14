using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using TextAdventure.Api.Models;
using TextAdventure.Api.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserStore>();

// JWT config uit appsettings (niet hardcoded)
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "textadventure-api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "textadventure-client";
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Jwt:Key ontbreekt. Zet environment variable Jwt__Key.");


builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetRequiredService<UserStore>();
    store.SeedDefaults(app.Configuration);
}



app.UseAuthentication();
app.UseAuthorization();

// -------- Helpers --------
static string Sha256(string input)
{
    var bytes = Encoding.UTF8.GetBytes(input);
    var hash = SHA256.HashData(bytes);
    return Convert.ToHexString(hash);
}

string CreateJwtToken(User user)
{
    var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub, user.Username),
    new(ClaimTypes.Name, user.Username),   // ?? DE FIX
    new(ClaimTypes.Role, user.Role)
};


    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// -------- Endpoints --------
app.MapGet("/health", () => Results.Ok("OK"));

app.MapPost("/api/auth/register", (RegisterRequest req, UserStore store) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        return Results.BadRequest("Username en password zijn verplicht.");

    if (req.Username.Length < 3 || req.Username.Length > 30)
        return Results.BadRequest("Username moet tussen 3 en 30 tekens zijn.");

    if (req.Password.Length < 6)
        return Results.BadRequest("Password moet minstens 6 tekens zijn.");

    if (store.Exists(req.Username))
        return Results.Conflict("Username bestaat al.");

    var role = (req.Role ?? "Player").Trim();
    role = role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Player";

    var user = new User
    {
        Username = req.Username.Trim(),
        PasswordHash = Sha256(req.Password),
        Role = role,
        FailedLoginCount = 0,
        IsLockedOut = false
    };

    store.Add(user);
    return Results.Created($"/api/auth/users/{user.Username}", new { user.Username, user.Role });
});

app.MapPost("/api/auth/login", (LoginRequest req, UserStore store) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        return Results.BadRequest("Username en password zijn verplicht.");

    var user = store.Get(req.Username.Trim());
    if (user is null)
        return Results.Unauthorized();

    if (user.IsLockedOut)
        return Results.StatusCode(423);

    var inputHash = Sha256(req.Password);

    if (!string.Equals(inputHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
    {
        user.FailedLoginCount++;
        if (user.FailedLoginCount >= 3)
            user.IsLockedOut = true;

        return Results.Unauthorized();
    }

    user.FailedLoginCount = 0;
    user.IsLockedOut = false;

    var token = CreateJwtToken(user);
    return Results.Ok(new { token, user = new { user.Username, user.Role } });
});

app.MapGet("/api/auth/me", (ClaimsPrincipal user) =>
{
    var username = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? user.Identity?.Name
                   ?? "unknown";

    var role = user.FindFirst(ClaimTypes.Role)?.Value
               ?? "unknown";

    return Results.Ok(new { username, role });
})
.RequireAuthorization();

app.MapGet("/api/keys/keyshare/{roomId}", (string roomId, ClaimsPrincipal user, IConfiguration config) =>
{
    // 1) JWT is verplicht (door RequireAuthorization hieronder), dus user heeft claims.
    var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "Player";

    // 2) Autorisatie-regel (projectconform):
    // - Player mag alleen keyshares voor "secret1" en "secret2" (of wat jij toelaat)
    // - Admin ("God") mag alle keyshares opvragen
    bool isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

    if (!isAdmin)
    {
        // Hier bepaal jij wat een Player mag.
        // Voor nu: enkel secret1 en secret2 zijn toegestaan.
        if (!string.Equals(roomId, "secret1", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(roomId, "secret2", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Forbid();
        }
    }

    // 3) Haal keyshare uit config (niet hardcoded in code)
    var keyshare = config[$"Keyshares:{roomId}"];

    if (string.IsNullOrWhiteSpace(keyshare))
        return Results.NotFound("Onbekende roomId of keyshare niet geconfigureerd.");

    return Results.Ok(new { roomId, keyshare });
})
.RequireAuthorization();





app.Run();
