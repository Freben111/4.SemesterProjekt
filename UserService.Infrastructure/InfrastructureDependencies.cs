using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("UserDatabase")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserQuery, UserQuery>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
