using ForumService.Application.Interfaces;
using ForumService.Domain.Interfaces;
using ForumService.Infrastructure.Database;
using ForumService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForumService.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ForumDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("ForumDatabase")));

            services.AddScoped<IForumRepository, ForumRepository>();
            services.AddScoped<IForumQuery, ForumQuery>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtValidator, JwtValidator>();

            return services;
        }
    }
}
