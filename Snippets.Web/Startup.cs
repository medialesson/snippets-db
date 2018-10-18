using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Security;
using NSwag.SwaggerGeneration.Processors.Security;
using NSwag;
using Snippets.Web.Common;
using Microsoft.AspNetCore.Http;
using Snippets.Web.Common.Middleware;

namespace Snippets.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MediatR and database services
            services.AddMediatR();
            services.AddEntityFrameworkSqlite().AddDbContext<SnippetsContext>(options =>
            {
                options.UseSqlite("Data Source=snippets.db");
            });

            services.BuildServiceProvider().GetRequiredService<SnippetsContext>().Database.EnsureCreated();

            // Bind settings to instance
            var settings = new AppSettings();
            Configuration.Bind("Snippets", settings);
            services.AddSingleton(Configuration);
            services.AddSingleton(settings);


            // Add auto mapper
            services.AddAutoMapper(GetType().Assembly);


            // Add common services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>(); // TODO: Blame Samuel in case it fucks up
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();


            // Add MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwagger();


            // Add auth
            services.AddJwt();
        }

        // Method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwaggerUi3WithApiExplorer(options =>
            {
                options.GeneratorSettings.Title = "Snippets API";
                options.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("jwt",
                    new NSwag.SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Scheme = "Bearer",
                        In = SwaggerSecurityApiKeyLocation.Header
                    })
                );

                options.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor("jwt"));
            });

            app.UseMvc();
        }
    }
}
