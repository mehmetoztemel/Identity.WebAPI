using Identity.WebAPI.Models;
using Identity.WebAPI.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.WebAPI.Services
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(AppUser user);
    }
    public class TokenService(IOptions<JwtOptions> _options, UserManager<AppUser> _userManager) : ITokenService
    {

        public async Task<string> GenerateToken(AppUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.Name,user.FullName)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));


            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string token = handler.WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
