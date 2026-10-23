using AFMA.Shared.Services;
using AFMA.Stats.Client;
using AFMA.Stats.Client.ModelBinders;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Keep worker threads ready so request bursts don't stall during thread-pool growth.
System.Threading.ThreadPool.SetMinThreads(100, 100);

builder.Services
  .AddRouting(options => options.LowercaseUrls = true)
  .AddSession(options => { options.IdleTimeout = TimeSpan.FromHours(3); })
  .AddAfmaApplicationBase(builder.Configuration, ab =>
  {
    ab
      .Add(ServiceActions.ApplicationServices,
        services => services.AddAfmaApplicationServices(builder.Configuration).AddControllersWithViews(options =>
          {
            options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
            options.ModelBinderProviders.Insert(1, new InvariantDateTimeModelBinderProvider());
          })
          .AddRazorRuntimeCompilation())
      .Add(ServiceActions.RazorPages, services => services.AddRazorPages())
      .Add(ServiceActions.NHibernate, services => services.AddAfmaNHibernate(builder.Configuration))
      .Add(ServiceActions.NotyfServices, services => services.AddNotyf())
      .Add(ServiceActions.AutoMapper, services => services.AddAfmaAutomapper(builder.Configuration))
      .Add(ServiceActions.ApplicationSettings, services => services.ConfigureApplicationSettings(builder.Configuration))
      .Add(ServiceActions.NotificationServices, services => services.AddAfmaNotificationServices(builder.Configuration))
      .Add(ServiceActions.DntCaptchaService, services => services.AddDNTCaptcha())
      .Add(ServiceActions.MaintenanceMode, services => services.AddAfmaMaintenanceMode(builder.Configuration));
  });

// Salmonella-module maintenance mode (settings + action filter). Recovered from the
// production build of 2026-07-21; this wiring was never committed.
MaintenanceExtensions.AddAfmaSalmonellaMaintenanceMode(builder.Services, builder.Configuration);

// Brute-force lockout for Identity sign-in.
builder.Services.Configure<IdentityOptions>(options =>
{
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
  options.Lockout.AllowedForNewUsers = true;
});

// Rate limiting: throttle login per-IP (brute force) and shed load globally so a flood returns 429
// instead of wedging the server with an unbounded request backlog.
builder.Services.AddRateLimiter(options =>
{
  options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
  options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
  {
    var path = httpContext.Request.Path.Value ?? string.Empty;
    if (path.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
    {
      var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
      return RateLimitPartition.GetFixedWindowLimiter("login:" + ip, _ => new FixedWindowRateLimiterOptions
      {
        // Generous per-IP cap so members behind a shared office IP (NAT) are not blocked;
        // targeted brute-force is handled by per-account lockout. This stops egregious floods/credential stuffing.
        PermitLimit = 100,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0,
      });
    }

    return RateLimitPartition.GetConcurrencyLimiter("global", _ => new ConcurrencyLimiterOptions
    {
      PermitLimit = 300,
      QueueLimit = 200,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    });
  });
});

// Persist Data Protection keys so auth cookies / antiforgery survive restarts AND are shared across
// instances (horizontal scale-out). For multi-node, point this directory at shared storage (NFS/blob)
// or switch to a Redis/DB key store; the ApplicationName must match across all instances of this app.
var dpKeyDir = builder.Configuration["DataProtection:KeyPath"] ?? "/var/afma/dpkeys-client";
builder.Services.AddDataProtection()
  .PersistKeysToFileSystem(new DirectoryInfo(dpKeyDir))
  .SetApplicationName("AFMA.Stats.Client");

var app = builder.Build();

app
  .ConfigureApplicationBase()
  .Run();
