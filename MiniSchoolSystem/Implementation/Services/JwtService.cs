using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Settings;
using MiniSchoolSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MiniSchoolSystem.Implementation.Services
{
    public class JwtService : iJwtService
    { private UserManager<UserDb> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly JwtSettings _jwtSettings;
public JwtService(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, ILogger logger, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateJwtToken(UserDb user)
        {
            

            string sectionJson= JsonSerializer.Serialize(user.UserSection);
            var roles = _userManager.GetRolesAsync(user);
            var roleForClaim = ((IEnumerable<string>)roles).FirstOrDefault() ?? "StudentOrTeacher";
            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim("Section",sectionJson),
                new Claim(ClaimTypes.Role,roleForClaim)
            };
          

            //Generate Private key for the Captured User

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Key!));
            var expires = DateTime.UtcNow.AddDays(7);
            //Signsture for approval of the Generate Key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Generate the Signedin Token
            var Token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, Claims, expires: expires, signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(Token);
            //
        }
    }
}
