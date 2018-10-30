﻿using System;
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
        /// <param name="jwtOptions">Options bound to the creation of the individual Jwt tokens</param>
        /// <param name="appSettings">Mapped version of "appsettings.json"</param>
        /// <param name="passwordHasher">Represents a type used to generate and verify passwords</param>
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

        #region Refresh Token Algorithm Notice
        /* 
         * The algorithm used to generate and verify 
         * a valid Refresh token works as elaborated below:
         *
         * Schema: rft.(payload).(checksum)
         *
         * The Refresh token has t anyways start with the prefix
         * "rft", this way we can ensure that the user is not
         * handing us a Jwt token. The payload is actually up to the
         * user of the algorithm, but we are using just a heavy amount of
         * random data here (256 bytes); its like a secret key and used to
         * make the checksum harder to break. At last there is a checksum,
         * which is generated via the use of the checksum of the currently 
         * valid Jwt token and the payload of the Refresh token. This way
         * the end-user has to have access to both tokens in order to
         * generate a new Jwt token. The Jwt token also has to be valid in
         * order to validate the Refresh token.
         *
         */
        #endregion

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