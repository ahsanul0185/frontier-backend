using FrontierWeb.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FrontierWeb.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrWhiteSpace(databaseUrl))
            {
                services.AddDbContext<BlogDbContext>(opt =>
                    opt.UseNpgsql(databaseUrl));
            }
            else
            {
                services.AddDbContext<BlogDbContext>(opt =>
                    opt.UseSqlite(cfg.GetConnectionString("BlogDb") ?? "Data Source=blog.db"));
            }

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }
    }
}
