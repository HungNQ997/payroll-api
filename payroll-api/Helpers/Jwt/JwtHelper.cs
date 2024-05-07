using Microsoft.IdentityModel.Tokens;
using payroll_api.Common.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace payroll_api.Helpers.Jwt
{
    public class JwtHelper : IJwtHelper
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SymmetricSecurityKey _key;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        public JwtHelper(AppSettings appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            _jwtSettings = appSettings.Jwt ?? throw new ArgumentNullException(nameof(appSettings.Jwt));
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public string CreateSecurityToken(DateTime expiresTime, IEnumerable<Claim> claims = null)
        {
            SigningCredentials signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                expires: expiresTime,
                signingCredentials: signingCredentials,
                claims: claims
                );

            return _tokenHandler.WriteToken(token);
        }

        public void ValidationToken(string token, out SecurityToken securityToken)
        {
            SecurityToken scToken;
            _tokenHandler.ValidateToken(token, _jwtSettings.GetTokenValidationParameters(), out scToken);
            securityToken = scToken;
        }
    }
}
