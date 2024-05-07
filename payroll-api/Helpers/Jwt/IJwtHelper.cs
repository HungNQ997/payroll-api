using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace payroll_api.Helpers.Jwt
{
    public interface IJwtHelper
    {
        string CreateSecurityToken(DateTime expiresTime, IEnumerable<Claim> claims = null);
        void ValidationToken(string token, out SecurityToken securityToken);
    }
}
