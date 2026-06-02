using MediaArchive.Application.Authentication.Interfaces;
using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.Interfaces;
using MediaArchive.Infrastructure.Persistence;
using MediaArchive.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaArchive.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")
            ));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IDownloadService, DownloadService>();

        return services;
    }
}