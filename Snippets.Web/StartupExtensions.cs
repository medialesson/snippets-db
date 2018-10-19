using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Snippets.Web.Common.Security;
using System;
using System.Text;

namespace Snippets.Web
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Validation service for Jwt tokens
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors</param>
        public static void AddJwt(this IServiceCollection services)
        {
            services.AddOptions();
            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            // Receive the secret key from the AppSettings
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Snippets").GetValue<string>("Secret")));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            string issuer = "snippets-api";
            string audience = "snippets";

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = signingCredentials;
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingCredentials.Key,

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}
