using System.Security.Cryptography;
using System.Text;
using TextAdventure.Api.Models;

namespace TextAdventure.Api.Storage
{
    public class UserStore
    {
        private readonly Dictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

        public bool Exists(string username) => _users.ContainsKey(username);

        public User? Get(string username)
        {
            _users.TryGetValue(username, out var user);
            return user;
        }

        public void Add(User user)
        {
            _users[user.Username] = user;
        }

        // Seed standaard users zodat je niet telkens PowerShell hoeft te doen
        public void SeedDefaults(IConfiguration config)
        {
            // Player seed (optioneel)
            var playerUser = config["Seed:PlayerUsername"];
            var playerPass = config["Seed:PlayerPassword"];

            if (!string.IsNullOrWhiteSpace(playerUser) && !string.IsNullOrWhiteSpace(playerPass))
                AddIfNotExists(playerUser, playerPass, "Player");

            // Admin seed (optioneel)
            var adminUser = config["Seed:AdminUsername"];
            var adminPass = config["Seed:AdminPassword"];

            if (!string.IsNullOrWhiteSpace(adminUser) && !string.IsNullOrWhiteSpace(adminPass))
                AddIfNotExists(adminUser, adminPass, "Admin");
        }


        private void AddIfNotExists(string username, string password, string role)
        {
            if (Exists(username)) return;

            Add(new User
            {
                Username = username,
                PasswordHash = Sha256(password),   // zelfde hashing als in Program.cs
                Role = role,
                FailedLoginCount = 0,
                IsLockedOut = false
            });
        }

        private static string Sha256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash); // uppercase hex, matcht jouw API
        }
    }
}
