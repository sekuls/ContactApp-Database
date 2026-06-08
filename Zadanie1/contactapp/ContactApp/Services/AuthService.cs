using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ContactApp.Data;
using ContactApp.DTOs;

namespace ContactApp.Services;

// uwirzytelnienie użytkowników. generuje JWT i sprawdze dane logowania
public class AuthService
{
    private readonly DbConfig _db;
    private readonly IConfiguration _config;

    public AuthService(DbConfig db, IConfiguration config)
    {
        _db     = db;
        _config = config;
    }

    //weryfikacja danych i generowanie tokenu
    public async Task<string?> LoginAsync(LoginRequest request)
    {
        //szukanie użytownika
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

        // sprawdzenie hasła
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return GenerateToken(user.Login, user.Id);
    }

    // generowanie jwt dla wybranego użytwnika
    private string GenerateToken(string login, int userId)
    {
        var key = _config["Jwt:Key"]!;
        var expireHours = int.Parse(_config["Jwt:ExpireHours"]!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.ASCII.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]{new Claim(ClaimTypes.Name,login),new Claim(ClaimTypes.NameIdentifier,userId.ToString())}),
            Expires = DateTime.UtcNow.AddHours(expireHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
