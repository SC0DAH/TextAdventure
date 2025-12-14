using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

public class AuthClient
{
    private readonly HttpClient _client;
    private string? _token;

    public string? Token => _token;
    public bool IsLoggedIn => !string.IsNullOrEmpty(_token);

    public AuthClient(string baseUrl = "http://localhost:5064") // server port http
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(baseUrl);
    }

    // registration
    public async Task<bool> RegisterAsync(string username, string password)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/register", new { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            Console.WriteLine("Network error during registration.");
            return false;
        }
    }

    // login
    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/login", new { Username = username, Password = password });
            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (!json.TryGetProperty("token", out var tokenElement)) return false;

            _token = tokenElement.GetString();
            return true;
        }
        catch
        {
            Console.WriteLine("Network error during login.");
            return false;
        }
    }

    // getting keyshare
    public async Task<string?> GetKeyShareAsync()
    {
        if (_token == null)
        {
            Console.WriteLine("You must login first.");
            return null;
        }

        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "/keyshare");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var resp = await _client.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            if (!json.TryGetProperty("keyshare", out var keyElement)) return null;

            return keyElement.GetString();
        }
        catch
        {
            Console.WriteLine("Network error during keyshare request.");
            return null;
        }
    }

    public void Logout()
    {
        _token = null;
    }
}


