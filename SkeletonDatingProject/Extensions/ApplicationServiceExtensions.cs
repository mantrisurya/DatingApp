using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkeletonDatingProject.Data;
using SkeletonDatingProject.Interfaces;
using SkeletonDatingProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options => { options.UseSqlite(config.GetConnectionString("DefaultConnection")); });

            return services; 
        }
    }
}
