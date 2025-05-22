using CommentService.Application.Interfaces;
using CommentService.Domain.Interfaces;
using CommentService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CommentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentService.Infrastructure.Security;

namespace CommentService.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<CommentDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("CommentDatabase")));

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentQuery, CommentQuery>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtValidator, JwtValidator>();

            return services;
        }
    }
}
