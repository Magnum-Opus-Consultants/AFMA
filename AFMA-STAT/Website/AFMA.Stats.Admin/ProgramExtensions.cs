namespace AFMA.Stats.Admin
{
  using AFMA.Shared.Services;
  using AspNetCoreHero.ToastNotification.Extensions;

  /// <summary>
  /// The Program Extensions Class.
  /// </summary>
  public static class ProgramExtensions
  {
    /// <summary>
    /// Configure The <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to Configure.</param>
    /// <returns>The COnfigured <see cref="WebApplication"/></returns>
    public static WebApplication ConfigureApplicationBase(this WebApplication app)
    {
      // Security response headers (applied to every response, including static files).
      app.Use(async (context, next) =>
      {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
        headers["Content-Security-Policy"] = "frame-ancestors 'self'";
        await next();
      });

      if (app.Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseAfmaMaintenanceMode();

      app.UseRouting();

      app.UseRateLimiter();

      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");

      app.MapRazorPages();
      app.UseNotyf();

      return app;
    }

    public static WebApplication MapAreas(this WebApplication app)
    {
      app.MapAreaControllerRoute(
        name: "GeneralArea",
        areaName: "General",
        pattern: "General/{controller=Home}/{action=Index}/{id?}");

      app.MapAreaControllerRoute(
        name: "SalmonellaArea",
        areaName: "Salmonella",
        pattern: "Salmonella/{controller=Home}/{action=Index}/{id?}");

      app.MapAreaControllerRoute(
        name: "MycotoxinArea",
        areaName: "Mycotoxin",
        pattern: "Mycotoxin/{controller=Home}/{action=Index}/{id?}");

      app.MapAreaControllerRoute(
        name: "IndustryProfileArea",
        areaName: "IndustryProfile",
        pattern: "IndustryProfile/{controller=Home}/{action=Index}/{id?}");

      app.MapAreaControllerRoute(
        name: "FeedsalesArea",
        areaName: "Feedsales",
        pattern: "Feedsales/{controller=Home}/{action=Index}/{id?}");

      return app;
    }
  }
}
