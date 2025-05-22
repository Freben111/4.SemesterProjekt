using PostService.Application.Interfaces;
using PostService.Domain.Interfaces;
using PostService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostService.Infrastructure.Security;

namespace PostService.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<PostDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("PostDatabase")));

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostQuery, PostQuery>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtValidator, JwtValidator>();

            return services;
        }
    }
}
