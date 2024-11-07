using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Catalogo_Api.Services;

public interface ItolkenService
{
    JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims, IConfiguration _config);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token,IConfiguration _config);

}
