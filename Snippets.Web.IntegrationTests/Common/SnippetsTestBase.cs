using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Snippets.Web.Common.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Snippets.Web.IntegrationTests.Common
{
    public class SnippetsTestBase : IDisposable
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private string _dbName = $"{Guid.NewGuid().ToString()}.db";

        public SnippetsTestBase()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Testing.json");

            var startup = new Startup(configBuilder.Build());
            var services = new ServiceCollection();

            services.AddDbContext<SnippetsContext>(options => options.UseSqlite(_dbName));

            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<SnippetsContext>().Database.EnsureCreated();
            _serviceScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
        }

        public void Dispose()
        {
            File.Delete(_dbName);
        }
    }
}
