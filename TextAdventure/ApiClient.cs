using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TextAdventure
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private string? _jwt;

        public ApiClient(string baseUrl)
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public void SetToken(string token)
        {
            _jwt = token;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/login", new
                {
                    username,
                    password
                });

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            catch
            {
                return null;
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
    }
}
