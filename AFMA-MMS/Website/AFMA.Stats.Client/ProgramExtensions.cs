namespace AFMA.Stats.Client
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
    /// <returns>The COnfigured <see cref="WebApplication"/></returns>
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
  }
}
