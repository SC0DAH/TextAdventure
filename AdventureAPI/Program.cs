using AdventureAPI.Services;
using AdventureAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdventureAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtKey = builder.Configuration["JwtKey"]; // halen jwtkey uit appsettings.json
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("JwtKey missing! Add it to appsettings.json or environment variable.");
            }
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)); // sleutel voor JWT gebruikt om tokens te ondertekenen en te verifiëren

            // DI / services
            builder.Services.AddSingleton<UserStore>();
            builder.Services.AddSingleton(new JwtService(jwtKey));
            builder.Services.AddSingleton<KeyService>();

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = signingKey,
                        ValidateIssuerSigningKey = true,
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middleware
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // /register, registreerd gebruiker
            app.MapPost("/register", (RegisterRequest req, UserStore store) =>
            {
                try
                {
                    if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                        return Results.BadRequest(new { error = "username and password required" });

                    var username = req.Username.Trim();

                    if (store.Users.ContainsKey(username))
                        return Results.BadRequest(new { error = "user already exists" });

                    var salt = PasswordHasher.GenerateSalt();
                    var hashed = PasswordHasher.HashWithSalt(req.Password, salt);

                    store.Users[username] = new UserRecord
                    {
                        Username = username,
                        PasswordHash = hashed,
                        Salt = salt
                    };

                    return Results.Ok(new { message = "registered" });
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // /login, returned jwt token bij succes, lockout na 5 gefaalde pogingen
            app.MapPost("/login", (LoginRequest req, UserStore store, JwtService jwt) =>
            {
                try
                {
                    if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                        return Results.BadRequest(new { error = "username and password required" });

                    var username = req.Username.Trim();

                    if (!store.Users.TryGetValue(username, out var user))
                        return Results.BadRequest(new { error = "invalid credentials" });

                    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                    {
                        var remaining = (int)(user.LockoutEnd.Value - DateTime.UtcNow).TotalSeconds;
                        return Results.BadRequest(new { error = "account locked", retry_seconds = remaining });
                    }

                    var providedHash = PasswordHasher.HashWithSalt(req.Password, user.Salt);
                    if (!PasswordHasher.SlowEquals(providedHash, user.PasswordHash))
                    {
                        user.FailedAttempts++;
                        if (user.FailedAttempts >= 5)
                        {
                            user.LockoutEnd = DateTime.UtcNow.AddSeconds(30);
                            user.FailedAttempts = 0;
                            return Results.BadRequest(new { error = "account locked due to failed attempts", retry_seconds = 30 });
                        }

                        return Results.BadRequest(new { error = "invalid credentials", attempts_left = 5 - user.FailedAttempts });
                    }

                    user.FailedAttempts = 0;
                    user.LockoutEnd = null;

                    var token = jwt.GenerateToken(username);

                    return Results.Ok(new { token });
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // /keyshare, genereerd Base64 encoded unieke 32-byte key
            app.MapGet("/keyshare", (HttpContext ctx, KeyService keyService) =>
            {
                try
                {
                    var username = ctx.User?.FindFirst("user")?.Value;
                    if (string.IsNullOrEmpty(username))
                        return Results.Unauthorized();

                    // Haal echte keyshare
                    var keyshare = keyService.GetOrCreateKeyForUser(username);
                    return Results.Ok(new { keyshare });
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            })
            .RequireAuthorization();

            app.Run();
        }
    }
}
