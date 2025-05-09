using UserService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserCommand, UserCommand>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserStateModelCommand, UserStateModelCommand>();
            return services;
        }
    }
}
