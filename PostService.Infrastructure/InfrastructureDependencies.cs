using PostService.Application.Interfaces;
using PostService.Domain.Interfaces;
using PostService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postervice.Infrastructure;
using PostService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<PostDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("ForumDatabase")));

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostQuery, PostQuery>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
