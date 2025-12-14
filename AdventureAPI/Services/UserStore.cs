using System.Collections.Concurrent;

namespace AdventureAPI.Services
{
    public class UserStore
    {
        public ConcurrentDictionary<string, UserRecord> Users { get; } = new();
    }

    public class UserRecord
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string Role { get; set; } = "Player";
    }

}
