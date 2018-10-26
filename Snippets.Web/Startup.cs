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
using Snippets.Web.Common.Services;
using FluentValidation.AspNetCore;
using Newtonsoft.Json;
using Snippets.Web.Common.Filter;
using Hangfire;

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
            // Bind settings to instance
            var appSettings = new AppSettings();
            Configuration.Bind("Snippets", appSettings);
            services.AddSingleton(Configuration);
            services.AddSingleton(appSettings);

            // Add MediatR
            services.AddMediatR();
            // Error handling for Fluent Validation within MediatR
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));


            // Add Entity Framework
            services.AddEntityFrameworkSqlite().AddDbContext<SnippetsContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.BuildServiceProvider().GetRequiredService<SnippetsContext>().Database.EnsureCreated();

            // Add Hangfire server
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));

            // Swagger
            services.AddSwagger();

            // Add MVC
            services.AddCors();
            services.AddMvc(opt => 
            {
                // Error handling for Fluent Validation
                opt.Filters.Add(typeof(ValidatorActionFilter));
            })
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Startup>());

            // Add auto mapper
            services.AddAutoMapper(GetType().Assembly);

            // Add common services
            services.AddSingleton<IMailService, SendGridMailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
            app.UseHttpsRedirection();

            // Error handling for user defined exceptions
            // BODGE: This is also used to do some returns as exceptions
            // we know that this is not the proper way of doing returns
            // but du to limited time and concentration this design 
            // decision was made ... we are sorry (it is your task to fix it)
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Hangfire background scheduler
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseCors(builder =>
                builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseMvc();
            app.UseStaticFiles();

            // Swagger (we are also supporting VisualStudio Code REST client)
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
        }
    }
}
