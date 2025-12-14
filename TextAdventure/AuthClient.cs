using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

public class AuthClient
{
    private readonly HttpClient _client;
    private string? _jwt;

    public bool IsLoggedIn => !string.IsNullOrEmpty(_jwt);
    public string? Role { get; private set; }
    public bool IsAdmin => Role == "Admin";

    public AuthClient(string baseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var payload = new { Username = username, Password = password };
        var res = await _client.PostAsJsonAsync("/auth/register", payload);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var payload = new { Username = username, Password = password };
        var res = await _client.PostAsJsonAsync("/auth/login", payload);

        if (!res.IsSuccessStatusCode) return false;

        var json = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (json == null || string.IsNullOrEmpty(json.Token)) return false;

        _jwt = json.Token;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);

        // Parse JWT en haal role
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(json.Token);
            Role = token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }
        catch
        {
            Role = null;
        }

        return true;
    }

    public async Task<string?> GetKeyShareAsync(string roomId = "room1")
    {
        if (_jwt == null) return null;

        var res = await _client.GetAsync($"/api/keys/keyshare/{roomId}");
        if (!res.IsSuccessStatusCode) return null;

        var json = await res.Content.ReadFromJsonAsync<KeyShareResponse>();
        return json?.Keyshare;
    }

    private class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }

    private class KeyShareResponse
    {
        [JsonPropertyName("keyshare")]
        public string Keyshare { get; set; } = string.Empty;
    }
}
