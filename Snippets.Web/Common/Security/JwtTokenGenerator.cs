using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Snippets.Web.Common.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        readonly JwtIssuerOptions _jwtOptions;
        
        readonly IPasswordHasher _passwordHasher;

        /// <summary>
        /// Initializes a JwtTokenGenerator
        /// </summary>
        /// <param name="jwtOptions">A set of option used to generate the jwt token</param>
        public JwtTokenGenerator(IOptions<JwtIssuerOptions> jwtOptions, IPasswordHasher passwordHasher)
        {
            _jwtOptions = jwtOptions.Value;
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
        public Task<string> CreateRefreshToken(string jwtToken, string payload)
        {
            return Task.Run(() => 
            {
                var tokenVerifySignature = jwtToken.Split('.')[2];
                
                if (string.IsNullOrEmpty(payload))
                {
                    var payloadBin = new byte[32];
                    using (var generator = RandomNumberGenerator.Create())
                    {
                        generator.GetBytes(payloadBin);
                    } 
                    payload = payloadBin.ToString();
                }

                var checksum = _passwordHasher.Hash(tokenVerifySignature, Encoding.UTF8.GetBytes(payload));

                return "${tokenVerifySignature}.${payload}.${checksum}";
            });
        }

    }
}