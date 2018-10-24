using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snippets.Web.Common.Extensions;

namespace Snippets.Web.Common.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        readonly JwtIssuerOptions _jwtOptions;
        readonly AppSettings _appSettings;
        readonly IPasswordHasher _passwordHasher;

        /// <summary>
        /// Initializes a JwtTokenGenerator
        /// </summary>
        /// <param name="jwtOptions"></param>
        /// <param name="appSettings"></param>
        /// <param name="passwordHasher"></param>
        public JwtTokenGenerator(IOptions<JwtIssuerOptions> jwtOptions, AppSettings appSettings, IPasswordHasher passwordHasher)
        {
            _jwtOptions = jwtOptions.Value;
            _appSettings = appSettings;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> CreateToken(string userId)
        {
            var claims = new Claim[]
            {
                // Claims which the token validates to
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

#if DEBUG
            // Validate tokens for one day while in debug mode
            _jwtOptions.ValidFor = TimeSpan.FromDays(1);
#endif

            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials
            );
            
            // Create a serialized token from the data above
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public Task<string> CreateRefreshToken(string jwtToken)
        {
            return Task.Run(() => 
            {
                if (jwtToken.StartsWith("rft"))
                    return null;

                var jwtSignature = jwtToken.Split('.')[2];

                byte[] payload = new byte[256]; // Payload of 256 bytes
                using (var generator = RandomNumberGenerator.Create())
                    generator.GetBytes(payload);

                var payloadBase64 = Convert.ToBase64String(payload).ToURLSave();
                var checksum = Convert.ToBase64String(_passwordHasher.Hash(jwtSignature, payload)).ToURLSave(); 
            
                return $"rft.{payloadBase64}.{checksum}";
            });
        }

        public Task<bool> VerifyRefreshToken(string refreshToken, string jwtToken)
        {
            return Task.Run(() =>
            {
                if (!refreshToken.StartsWith("rft"))
                    return false;

                var refreshTokenParts = refreshToken.Split('.');
                var refreshTokenPayload = Convert.FromBase64String(refreshTokenParts[1].FromURLSave());
                var refreshTokenChecksum = refreshTokenParts[2];

                var jwtSignature = jwtToken.Split('.')[2];
                
                var validationChecksum = Convert.ToBase64String(new PasswordHasher(_appSettings).Hash(jwtSignature, refreshTokenPayload)).ToURLSave();
                if (validationChecksum == refreshTokenChecksum)
                    return true;
                else
                    return false;
            });
        }
    }
}