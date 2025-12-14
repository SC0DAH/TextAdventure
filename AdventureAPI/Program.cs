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
            app.MapPost("/auth/register", (RegisterRequest req, UserStore store) =>
            {
                try
                {
                    if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                        return Results.BadRequest(new { error = "username and password required" });

                    var username = req.Username.Trim();

                    if (store.Users.ContainsKey(username))
                        return Results.BadRequest(new { error = "user already exists" });

                    // salt + hash
                    var salt = PasswordHasher.GenerateSalt();
                    var hashedPassword = PasswordHasher.HashWithSalt(req.Password, salt);

                    var role = req.Role?.Trim() == "Admin" ? "Admin" : "Player";

                    store.Users[username] = new UserRecord
                    {
                        Username = username,
                        PasswordHash = hashedPassword,
                        Salt = salt,
                        Role = role
                    };

                    return Results.Ok(new { message = "registered", role });
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });


            // /login, returned jwt token bij succes, lockout na 5 gefaalde pogingen
            app.MapPost("/auth/login", (LoginRequest req, UserStore store, JwtService jwt) =>
            {
                try
                {
                    if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                        return Results.BadRequest(new { error = "username and password required" });

                    var username = req.Username.Trim();

                    // bestaat user?
                    if (!store.Users.TryGetValue(username, out var user))
                        return Results.BadRequest(new { error = "invalid credentials" });

                    // checken op lockout
                    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                    {
                        var remaining = (int)(user.LockoutEnd.Value - DateTime.UtcNow).TotalSeconds;
                        return Results.BadRequest(new { error = "account locked", retry_seconds = remaining });
                    }

                    // hash vergelijken
                    var providedHash = PasswordHasher.HashWithSalt(req.Password, user.Salt);
                    if (!PasswordHasher.SlowEquals(providedHash, user.PasswordHash))
                    {
                        user.FailedAttempts++;

                        // lockout
                        if (user.FailedAttempts >= 3)
                        {
                            user.LockoutEnd = DateTime.UtcNow.AddSeconds(30); // 30 sec
                            user.FailedAttempts = 0;
                            return Results.BadRequest(new { error = "account locked due to failed attempts", retry_seconds = 30 });
                        }

                        return Results.BadRequest(new { error = "invalid credentials", attempts_left = 3 - user.FailedAttempts });
                    }

                    user.FailedAttempts = 0;
                    user.LockoutEnd = null;

                    var token = jwt.GenerateToken(username, user.Role);

                    return Results.Ok(new
                    {
                        token,
                        username = user.Username,
                        role = user.Role
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/auth/me", (HttpContext ctx) =>
            {
                // check of de user geauthenticeerd is
                if (!ctx.User.Identity?.IsAuthenticated ?? true)
                    return Results.Unauthorized();

                var username = ctx.User.FindFirst("user")?.Value;
                var role = ctx.User.FindFirst("role")?.Value;

                if (username == null || role == null)
                    return Results.Unauthorized();

                return Results.Ok(new
                {
                    username,
                    role
                });
            }).RequireAuthorization();

            // /keyshare, genereerd Base64 encoded unieke 32-byte key
            app.MapGet("/api/keys/keyshare/{roomId}", (string roomId, HttpContext ctx, KeyService keyService) =>
            {
                try
                {
                    var username = ctx.User?.FindFirst("user")?.Value;
                    var role = ctx.User?.FindFirst("role")?.Value;

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                        return Results.Unauthorized();

                    // check permissie
                    if (role != "Player" && role != "Admin")
                        return Results.Forbid();

                    // key ophalen of aanmaken per room
                    var keyshare = keyService.GetOrCreateKeyForUser(roomId, username);

                    return Results.Ok(new
                    {
                        roomId,
                        username,
                        role,
                        keyshare
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.Run();
        }
    }
}
