using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProductManagement.Infrastructure.Services
{
    // AuthService = Kimlik doğrulama ve token işlemleri
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        // JWT ayarları
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationMinutes;

        // Constructor - Konfigürasyonları al
        public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // appsettings.json'dan JWT ayarlarını al
            _jwtSecret = _configuration["Jwt:Secret"] ?? throw new ArgumentNullException("JWT Secret is required");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? "ProductManagementAPI";
            _jwtAudience = _configuration["Jwt:Audience"] ?? "ProductManagementAPI";
            _jwtExpirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "1440"); // Default: 24 saat
        }

        // JWT Token oluştur
        public async Task<string> GenerateJwtTokenAsync(int userId, string email, string role)
        {
            try
            {
                _logger.LogInformation("Generating JWT token for user: {Email}", email);

                // Token içinde saklanacak bilgiler (Claims)
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, userId.ToString()), // Subject = User ID
                    new(JwtRegisteredClaimNames.Email, email),           // Email
                    new(ClaimTypes.Role, role),                          // Rol (Admin, User vb.)
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique)
                    new(JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64) // Issued At
                };

                // Gizli anahtar ile imzalama
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Token ayarları
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),    // Token içindeki bilgiler
                    Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes), // Geçerlilik süresi
                    Issuer = _jwtIssuer,                     // Token'ı kim oluşturdu
                    Audience = _jwtAudience,                 // Token kime yönelik
                    SigningCredentials = credentials         // İmzalama bilgileri
                };

                // Token'ı oluştur
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Token'ı string'e çevir
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("JWT token generated successfully for user: {Email}", email);

                return await Task.FromResult(tokenString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user: {Email}", email);
                throw;
            }
        }

        // JWT Token doğrula
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _logger.LogDebug("Validating JWT token");

                var tokenHandler = new JwtSecurityTokenHandler();

                // Validation parametrelerini ayarla
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,         // İmzayı doğrula
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                    ValidateIssuer = true,                   // Issuer'ı doğrula
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,                 // Audience'ı doğrula
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,                 // Geçerlilik süresini kontrol et
                    ClockSkew = TimeSpan.Zero                // Zaman farklılığı toleransı
                };

                // Token'ı doğrula
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Token başarıyla doğrulandı
                _logger.LogDebug("JWT token validated successfully");
                return await Task.FromResult(true);
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT token has expired");
                return await Task.FromResult(false);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed");
                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
                return await Task.FromResult(false);
            }
        }

        // Password'ü hash'le (bcrypt benzeri güvenli hashing)
        public string HashPassword(string password)
        {
            try
            {
                // Salt oluştur (her password için farklı)
                byte[] salt = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                // PBKDF2 ile hash oluştur
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                // Salt + Hash'i birleştir
                byte[] hashBytes = new byte[48];
                Array.Copy(salt, 0, hashBytes, 0, 16);  // İlk 16 byte = salt
                Array.Copy(hash, 0, hashBytes, 16, 32); // Son 32 byte = hash

                // Base64 string'e çevir
                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hashing password");
                throw;
            }
        }

        // Password'ü doğrula
        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Base64'ten byte array'e çevir
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // Salt'ı ayır (ilk 16 byte)
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                // Mevcut hash'i ayır (son 32 byte)
                byte[] storedHash = new byte[32];
                Array.Copy(hashBytes, 16, storedHash, 0, 32);

                // Girilen password'ü aynı salt ile hash'le
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] computedHash = pbkdf2.GetBytes(32);

                // Hash'leri karşılaştır (timing attack'e karşı güvenli)
                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }
    }
}
