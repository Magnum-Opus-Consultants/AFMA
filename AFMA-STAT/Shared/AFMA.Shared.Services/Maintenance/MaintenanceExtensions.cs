namespace AFMA.Shared.Services;

using AFMA.Shared.Services.Maintenance;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DI and pipeline helpers for the maintenance-mode middleware.
/// </summary>
public static class MaintenanceExtensions
{
  public static IServiceCollection AddAfmaMaintenanceMode(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddOptions();
    services.Configure<MaintenanceSettings>(configuration.GetSection(MaintenanceSettings.SectionName));
    return services;
  }

  /// <summary>
  /// Registers the Salmonella-module maintenance settings and the action filter that enforces them.
  /// Recovered from the production build of 2026-07-21; this code was never committed.
  /// </summary>
  public static IServiceCollection AddAfmaSalmonellaMaintenanceMode(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<SalmonellaMaintenanceSettings>(configuration.GetSection(SalmonellaMaintenanceSettings.SectionName));
    services.AddScoped<SalmonellaMaintenanceFilter>();
    return services;
  }

  public static IApplicationBuilder UseAfmaMaintenanceMode(this IApplicationBuilder app)
  {
    return app.UseMiddleware<MaintenanceMiddleware>();
  }
}
