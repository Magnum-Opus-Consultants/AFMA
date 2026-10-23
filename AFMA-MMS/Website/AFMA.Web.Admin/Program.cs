using AFMA.Shared.Services;
using AFMA.Web.Admin;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddRouting(options => options.LowercaseUrls = true)
  .AddSession(options => { options.IdleTimeout = TimeSpan.FromHours(3); })
  .AddAfmaApplicationBase(builder.Configuration, ab =>
  {
    ab
      .Add(ServiceActions.WebApplicationServices, services => services.AddAfmaWimsbApplicationServices(builder.Configuration)
        .AddControllersWithViews().AddRazorRuntimeCompilation())
      .Add(ServiceActions.RazorPages, services => services.AddRazorPages())
      .Add(ServiceActions.NHibernate, services => services.AddAfmaWimsNHibernate(builder.Configuration))
      .Add(ServiceActions.NotyfServices, services => services.AddNotyf())
      .Add(ServiceActions.AutoMapper, services => services.AddAfmaAutomapper(builder.Configuration))
      .Add(ServiceActions.ApplicationSettings, services => services.ConfigureApplicationSettings(builder.Configuration))
      .Add(ServiceActions.NotificationServices, services => services.AddAfmaNotificationServices(builder.Configuration))
      .Add(ServiceActions.Caching, services => services.AddCaching())
      .Add(ServiceActions.Serilog, services => services.AddAfmaLogging(builder.Configuration));
  });

var app = builder.Build();
app.UseSession();
app
  .MapAreas()
  .ConfigureApplicationBase()
  .Run();