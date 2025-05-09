using ForumService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<IForumCommand, ForumCommand>();
            services.AddScoped<IForumStateModelCommand, ForumStateModelCommand>();
            return services;
        }
    }
}
