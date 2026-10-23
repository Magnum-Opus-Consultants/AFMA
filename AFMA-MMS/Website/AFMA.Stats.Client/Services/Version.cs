namespace AFMA.Stats.Client.Services
{
  using System.Reflection;

  public static class Version
  {
    public static string ApplicationVersion()
    {
      var asm = Assembly.GetExecutingAssembly();
      var version = asm.GetName().Version;
      var product = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), true).FirstOrDefault() as AssemblyProductAttribute;

      var compileDate = File.GetLastWriteTime(asm.Location);

      if (version != null && product != null)
      {
        return
          $"<span><strong>{product.Product}</strong>: {version.Major}.{version.Minor}.{version.Build} (Revision: {version.Revision}) | Build Date: {compileDate:dd/MM/yyyy HH:mm:ss}</span>";
      }

      return string.Empty;
    }

    public static string ApplicationVersionShort()
    {
      var version = Assembly.GetExecutingAssembly().GetName().Version;

      if (version != null)
      {
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
      }

      return string.Empty;
    }
  }
}
