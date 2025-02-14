

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApp.Model;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TestApp.Services
    {
        public class JwtService
        {
            private readonly IConfiguration _configuration;

            public JwtService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public string GenerateToken(Users user)
            {
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.EmailOrPhoneNumber),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }

