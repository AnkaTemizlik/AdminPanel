using DNA.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DNA.API.Infrastructure {
    public interface IJwtFactory {
        Task<string> GenerateEncodedToken(ApplicationUser user);
    }
    public class JwtFactory : IJwtFactory {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IConfiguration _configuration;

        public JwtFactory(IConfiguration configuration, IOptions<JwtIssuerOptions> jwtOptions) {
            _jwtOptions = jwtOptions.Value;
            _configuration = configuration;
        }

        public async Task<string> GenerateEncodedToken(ApplicationUser user) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
                new Claim(ClaimTypes.Name, $"{user.Id}"),
                new Claim(ClaimTypes.Email, user.Email)
            };
            claims.AddRange(user.Roles.Select(_ => new Claim(ClaimTypes.Role, _)));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration.GetSection("HttpsRedirection").GetValue<bool>("Enabled") ? _configuration["Origins:Https"] : _configuration["Origins:WEB"],
                claims: claimsIdentity.Claims,
                expires: DateTime.Now.AddSeconds(user.ExpiresIn),
                signingCredentials: credentials
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

    }
}
