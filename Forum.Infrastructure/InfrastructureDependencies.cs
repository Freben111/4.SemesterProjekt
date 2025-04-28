using ForumService.Application.Interfaces;
using ForumService.Domain.Interfaces;
using ForumService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return services;
        }
    }
}
