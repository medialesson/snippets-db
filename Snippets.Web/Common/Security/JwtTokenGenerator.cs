﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Snippets.Web.Common.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        readonly JwtIssuerOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> CreateToken(string userId)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

#if DEBUG
            // Validate tokens for one day
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
            
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}