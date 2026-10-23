namespace AFMA.Web.Admin
{
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
    /// <returns>The Configured <see cref="WebApplication"/></returns>
    public static WebApplication ConfigureApplicationBase(this WebApplication app)
    {
      if (app.Environment.IsDevelopment())
      {
      }
      else
      {
        app.UseExceptionHandler("/Error");
        //app.UseStatusCodePagesWithReExecute("/Error/{0}");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

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
        name: "ProfileArea",
        areaName: "Profile",
        pattern: "Profile/{controller=Home}/{action=Index}/{id?}");

      //app.MapAreaControllerRoute(
      //  name: "SalmonellaArea",
      //  areaName: "Salmonella",
      //  pattern: "Salmonella/{controller=Home}/{action=Index}/{id?}");

      //app.MapAreaControllerRoute(
      //  name: "MycotoxinArea",
      //  areaName: "Mycotoxin",
      //  pattern: "Mycotoxin/{controller=Home}/{action=Index}/{id?}");

      //app.MapAreaControllerRoute(
      //  name: "IndustryProfileArea",
      //  areaName: "IndustryProfile",
      //  pattern: "IndustryProfile/{controller=Home}/{action=Index}/{id?}");

      //app.MapAreaControllerRoute(
      //  name: "FeedsalesArea",
      //  areaName: "Feedsales",
      //  pattern: "Feedsales/{controller=Home}/{action=Index}/{id?}");

      return app;
    }

  }
}
