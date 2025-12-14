using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TextAdventure.Security;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(string baseUrl)
    {
        var handler = new HttpClientHandler();

#if DEBUG
        // Dev-only: lokaal HTTPS dev cert issues omzeilen
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
        };
    }


    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            var req = new LoginRequest { Username = username, Password = password };
            var res = await _http.PostAsJsonAsync("api/auth/login", req);

            if (!res.IsSuccessStatusCode)
            {
                // 401 / 423 / 400 etc.
                return null;
            }

            var body = await res.Content.ReadFromJsonAsync<LoginResponse>();
            if (body == null || string.IsNullOrWhiteSpace(body.Token)) return null;

            return body;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetKeyshareAsync(string roomId, string jwt)
    {
        try
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            var res = await _http.GetAsync($"api/keys/keyshare/{roomId}");
            if (!res.IsSuccessStatusCode) return null;

            var body = await res.Content.ReadFromJsonAsync<KeyshareResponse>();
            return string.IsNullOrWhiteSpace(body?.Keyshare) ? null : body!.Keyshare;
        }
        catch
        {
            return null;
        }
    }
}
