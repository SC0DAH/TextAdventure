namespace TextAdventure
{
    public class AuthService
    {
        private readonly ApiClient _api;

        public AuthService(ApiClient api)
        {
            _api = api;
        }

        public async Task<bool> LoginAsync()
        {
            Console.WriteLine("=== LOGIN REQUIRED ===");

            Console.Write("Username: ");
            var username = Console.ReadLine()?.Trim();

            Console.Write("Password: ");
            var password = ReadPassword();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Invalid input.");
                return false;
            }

            var token = await _api.LoginAsync(username, password);
            if (token == null)
            {
                Console.WriteLine("Login failed.");
                return false;
            }

            _api.SetToken(token);
            Console.WriteLine("Login successful!");
            return true;
        }

        private static string ReadPassword()
        {
            var pwd = "";
            ConsoleKey key;

            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd = pwd[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl((char)key))
                {
                    pwd += (char)key;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd;
        }
    }
}
