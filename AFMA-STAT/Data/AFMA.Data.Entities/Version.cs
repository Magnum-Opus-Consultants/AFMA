namespace AFMA.Data.Entities
{
  using System.Linq;
  using System.Reflection;

  public static class Version
  {
    public static string ApplicationVersion()
    {
      var asm = Assembly.GetExecutingAssembly();
      var version = asm.GetName().Version;
      var product = asm.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), true).FirstOrDefault() as System.Reflection.AssemblyProductAttribute;

      var compileDate = File.GetLastWriteTime(asm.Location);

      if (version != null && product != null)
      {
        return
          $"<span><strong>{product.Product}</strong>: {version.Major}.{version.Minor}.{version.Build} (Revision: {version.Revision}) | Build Date: {compileDate:dd/MM/yyyy HH:mm:ss}</span>";
      }
      else
      {
        return string.Empty;
      }
    }
  }
}
