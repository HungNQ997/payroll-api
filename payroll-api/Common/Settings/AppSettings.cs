using Microsoft.IdentityModel.Tokens;
using Payroll.Core.Settings;

namespace payroll_api.Common.Settings
{
    public class AppSettings
    {
        public JwtSettings Jwt { get; set; }
        public MongoDBSettings MongoDBSettings { get; set; }
        public string? RedisSessionConfig { get; set; }
    }

    public class AuthUser
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? SecurityKey { get; set; }
    }

    public class JwtSettings
    {
        private bool _tokenValidationParametersBinded = false;

        private TokenValidationParameters _tokenValidationParameters;

        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }

        public void bindTokenValidationParams(TokenValidationParameters tokenValidationParameters)
        {
            if (_tokenValidationParametersBinded)
            {
                return;
            }

            _tokenValidationParameters = tokenValidationParameters;
            _tokenValidationParametersBinded = true;
        }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            return _tokenValidationParameters ??
                throw new NullReferenceException("Make sure you call bindTokenValidationParams before GetTokenValidationParameters");
        }
    }
}
