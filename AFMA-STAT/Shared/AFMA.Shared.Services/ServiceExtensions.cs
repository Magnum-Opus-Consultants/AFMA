namespace AFMA.Shared.Services;
using AFMA.ApplicationLayer;
using AFMA.ApplicationLayer.Interface;
using AFMA.ApplicationLayer.Services;
using AFMA.AutoMapping;
using AFMA.Data.Entities;
using AFMA.Data.Entities.Wims;
using AFMA.Data.Mappings;
using AFMA.Data.Mappings.Wims;
using AFMA.Infrastructure.Options;
using AFMA.Infrastructure.Settings;
using AFMA.Shared.Infrastructure.Settings;
using Alpha.Services.Core.Builders;
using Alpha.Services.NHibernate.Repository.Configuration;
using Alpha.Services.Notifications.Configuration;
using Alpha.Services.Notifications.Templates.Configuration;
using AspNetCoreHero.ToastNotification;
using AutoMapper;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Filters;

/// <summary>
/// Service Extensions Class.
/// </summary>
public static class ServiceExtensions
{
  /// <summary>
  /// Configures the application base.
  /// This would contain service configuration that might be shared between multiple projects.
  /// </summary>
  /// <param name="services">The configured services.</param>
  /// <param name="configuration">The Application Configuration.</param>
  /// <param name="actionBuilder">The Action Builder.</param>
  /// <returns>The configured Services.</returns>
  public static IServiceCollection AddAfmaApplicationBase(this IServiceCollection services, IConfiguration configuration, Action<ServiceCollectionBuilder> actionBuilder = null!)
  {
    var builder = new ServiceCollectionBuilder(services);
    actionBuilder?.Invoke(builder);
    builder.Build();

    return services;
  }

  /// <summary>
  /// Configures the Application Services.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  /// <param name="configuration"> System Configuration.</param>
  /// <returns>
  /// <see cref="IServiceCollection"/> Current services including the newly added services.
  /// </returns>
  public static IServiceCollection AddAfmaApplicationServices(this IServiceCollection services, IConfiguration configuration)
  {
    return services
      .AddScoped<IApplicationLayer, ApplicationLayer>()
      .AddScoped<IMycotoxinLayer, MycotoxinLayer>()
      .AddScoped<IIdentityLayer, IdentityLayer>()
      .AddScoped<ISalmonellaLayer, SalmonellaLayer>()
      .AddScoped<IFeedsalesLayer, FeedsalesLayer>()
      .AddScoped<ILookupLayer, LookupLayer>()
      .AddScoped<IIndustryProfileLayer, IndustryProfileLayer>()
      .AddScoped<INotificationService, NotificationService>()
      .AddScoped<IExternalApiService, ExternalApiService>()
      .AddScoped<IExternalApiLayer, ExternalApiLayer>();
  }

  /// <summary>
  /// Configures the Application Services.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  /// <param name="configuration"> System Configuration.</param>
  /// <returns>
  /// <see cref="IServiceCollection"/> Current services including the newly added services.
  /// </returns>
  public static IServiceCollection AddAfmaNotificationServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.ConfigureAlphaNotificationTemplates();
    return services.ConfigureAlphaEmailNotificationService(options =>
    {
      options.Username = configuration["SMTPSettings:Username"];
      options.Password = configuration["SMTPSettings:Password"];
      options.FromAddress = configuration["SMTPSettings:FromAddress"];
      options.FromName = configuration["SMTPSettings:FromName"];
      options.Host = configuration["SMTPSettings:Host"];
      options.Port = int.Parse(configuration["SMTPSettings:Port"]);
    });
  }

  /// <summary>
  /// Configures the Application to use NHibernate.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  /// <param name="configuration"> System Configuration.</param>
  /// <returns>
  /// <see cref="IServiceCollection"/> Current services including the newly added services.
  /// </returns>
  public static IServiceCollection AddAfmaNHibernate(this IServiceCollection services, IConfiguration configuration)
  {
    services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
    {
      options.LoginPath = "/Identity/Account/Login";
      options.LogoutPath = "/Identity/Account/Logout";
      options.ExpireTimeSpan = TimeSpan.FromMinutes(180);
      options.SlidingExpiration = true;
    });

    services.ConfigureNHibernateRepository<ApplicationUser, SessionFactory>(services.BuildServiceProvider(), configuration, typeof(MappingModule).Assembly);

    return services;
  }

  public static IServiceCollection AddAfmaWimsNHibernate(this IServiceCollection services, IConfiguration configuration)
  {
    services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
    {
      options.LoginPath = "/Identity/Account/Login";
      options.LogoutPath = "/Identity/Account/Logout";
      options.ExpireTimeSpan = TimeSpan.FromMinutes(180);
      options.SlidingExpiration = true;
    });

    services.ConfigureNHibernateRepository<WimsApplicationUser, WimsSessionFactory>(services.BuildServiceProvider(), configuration, typeof(WimsMappingModule).Assembly);

    return services;
  }

  /// <summary>
  /// Configures the Application and other Settings.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  /// <param name="configuration"> System Configuration.</param>
  /// <returns>
  /// <see cref="IServiceCollection"/> Current services including the newly added services.
  /// </returns>
  public static IServiceCollection ConfigureApplicationSettings(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddOptions();
    services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
    services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

    return services;
  }

  /// <summary>
  /// Configures the Application to use Notify Toast services.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  public static void AddNotyf(this IServiceCollection services)
  {
    services.AddNotyf(config =>
    {
      config.DurationInSeconds = 10;
      config.IsDismissable = true;
      config.Position = NotyfPosition.TopRight;
    });
  }

  /// <summary>
  /// Configures the DNT Captcha services.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  public static IServiceCollection AddDNTCaptcha(this IServiceCollection services)
  {
    services.AddDNTCaptcha(options =>
      options.UseCookieStorageProvider().ShowThousandsSeparators(false)
        .WithEncryptionKey("edaa1233179e4cc09fac9040894a7fba"));

    return services;
  }

  /// <summary>
  /// Configures the Caching service.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  public static IServiceCollection AddCaching(this IServiceCollection services)
  {
    services.AddMemoryCache();
    return services;
  }

  /// <summary>
  /// Configures the Application to use Automapper.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  /// <param name="configuration"> System Configuration.</param>
  /// <returns>
  /// <see cref="IServiceCollection"/> Current services including the newly added services.
  /// </returns>
  public static IServiceCollection AddAfmaAutomapper(this IServiceCollection services, IConfiguration configuration)
  {
    var autoMappings = new[]
    {
      typeof(AutoMappingModule).Assembly,
    };

    var mappingConfig = new MapperConfiguration(mc =>
    {
      mc.AddMaps(autoMappings);
    });

    var mapper = mappingConfig.CreateMapper();

    return services
      .AddSingleton(mapper);
  }

  public static IServiceCollection AddAfmaLogging(this IServiceCollection services, IConfiguration configuration)
  {
    var loggerConfig = new LoggerConfiguration()
      // serilog-aspnetcore picks up and logs exceptions, so we filter out logs from the standard middleware to prevent duplicate logs.
      .Filter.ByExcluding(Matching.FromSource<Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware>())
      .Filter.ByExcluding(Matching.FromSource<Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware>())
      // Filters out 'info' logs from Microsoft.AspNetCore when the request path is a health check URL
      .Filter.ByExcluding(Matching.WithProperty<string>(
        propertyName: "RequestPath",
        requestPath => requestPath.EndsWith("/health/live", StringComparison.InvariantCultureIgnoreCase) || requestPath.EndsWith("/health/ready", StringComparison.InvariantCultureIgnoreCase)))
      .Enrich.FromLogContext()
      .ReadFrom.Configuration(configuration);

    Log.Logger = loggerConfig.CreateLogger();

    services.AddSerilog();

    //services.AddLogCleanupForSqlServer<ModelDbContext>(configuration);

    return services;
  }

  public static IServiceCollection AddAfmaCaching(this IServiceCollection services)
  {
    services.AddMemoryCache();

    services.Configure<MemoryCacheEntryOptions>(options =>
    {
      options.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
      options.SlidingExpiration = TimeSpan.FromDays(30);
    });

    return services;
  }

  #region AFMA Website

  /// <summary>
  /// Configures the application base.
  /// This would contain service configuration that might be shared between multiple projects.
  /// </summary>
  /// <param name="services">The configured services.</param>
  /// <param name="configuration">The Application Configuration.</param>
  /// <param name="actionBuilder">The Action Builder.</param>
  /// <returns>The configured Services.</returns>
  public static IServiceCollection AddAfmaWebApplicationBase(this IServiceCollection services, IConfiguration configuration, Action<ServiceCollectionBuilder> actionBuilder = null!)
  {
    var builder = new ServiceCollectionBuilder(services);
    actionBuilder?.Invoke(builder);
    builder.Build();

    return services;
  }

  /// <summary>
  /// Configures the Application Services.
  /// </summary>
  /// <param name="services"> Where the services get added.</param>
  /// <param name="configuration"> System Configuration.</param>
  /// <returns>
  /// <see cref="IServiceCollection"/> Current services including the newly added services.
  /// </returns>
  public static IServiceCollection AddAfmaWimsbApplicationServices(this IServiceCollection services, IConfiguration configuration)
  {
    return services
      .AddScoped<IWimsApplicationLayer, WimsApplicationLayer>()
      //.AddScoped<IApplicationLayer, ApplicationLayer>()
      .AddScoped<IWimsIdentityLayer, WimsIdentityLayer>();
  }

  #endregion
}
