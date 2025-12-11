using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdventureAPI
{
    public class JwtService
    {
        private readonly string _key;
        public JwtService(string key) => _key = key;

        public string GenerateToken(string username, int expireHours = 1)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)); // symmetrische sleutel = gebruikt voor signeren en verifieren
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // ondertekeningsmethode

            var claims = new[] {new Claim("user", username)}; // info die in token opgslagen wordt

            var token = new JwtSecurityToken( // jwt token maken met claims, vervaldatum en ondertekingen met key
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token); // zet token om naar string die je kan gebruiken als authorization header
        }
    }
}
