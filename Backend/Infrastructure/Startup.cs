using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure
{

    /// <summary>
    /// Setup of dependency injections, the methods are called from Program.cs in WebApi
    /// </summary>
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var host = Environment.GetEnvironmentVariable("MYSQL_HOST");
            //var port = Environment.GetEnvironmentVariable("MYSQL_PORT");
            var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
            var user = Environment.GetEnvironmentVariable("MYSQL_USER");
            var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            var connectionString = $"server={host};user={user};database={database};password={password};";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 34));
            return services.AddDbContext<CrawlerDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion)
                    // The following three options help with debugging, but should
                    // be changed or removed for production.
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            ).AddScoped<IWebsiteRecordRepository, WebsiteRecordRepository>();
        }
    }
}