using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WorldModel;

namespace comp584server
{
    public class JwtHandler(UserManager<WorldModelUser> userManager, IConfiguration configuration)
    {
        public async Task<JwtSecurityToken> GenerateTokenAsync(WorldModelUser user)
        {
            return new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpiryInMinutes"])),
                claims : await GetClaimsAsync(user),
                signingCredentials : GetSigningCredentials()
            );
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] key = Convert.FromBase64String(configuration["JwtSettings:Secretkey"]!);
            SymmetricSecurityKey secret = new (key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaimsAsync(WorldModelUser user)
        {
            List<Claim> claims = [new Claim(ClaimTypes.Name, user.UserName!)];
            //claims.AddRange(await userManager.GetRolesAsync(user)
            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
