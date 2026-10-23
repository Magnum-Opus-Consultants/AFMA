namespace AFMA.Stats.Admin.Services
{
  using System.Reflection;
  using System.Text;

  [Obsolete("Not used",true)]
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
      //var version = Assembly.GetExecutingAssembly().GetName().Version;

      //if (version != null)
      //{
      //  return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
      //}

      return string.Empty;
    }

    public static string ApplicationGitVersion()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var assemblyName = assembly.GetName().Name;
      var gitVersionInformationType = assembly.GetType("GitVersionInformation");
      var fields = gitVersionInformationType.GetFields();
      var sb = new StringBuilder();

      foreach (var field in fields)
      {
        sb.Append(field.Name);
        sb.Append(field.GetValue(null));
        sb.Append(".");
        //Trace.WriteLine(string.Format("{0}: {1}", field.Name, field.GetValue(null)));
      }

      // The GitVersionInformation class generated from a F# project exposes properties
      var properties = gitVersionInformationType.GetProperties();

      foreach (var property in properties)
      {
        sb.Append(property.Name);
        sb.Append(property.GetGetMethod(true).Invoke(null, null));
        sb.Append(".");

        //Trace.WriteLine(string.Format("{0}: {1}", property.Name, property.GetGetMethod(true).Invoke(null, null)));
      }

      return sb.ToString();
    }
  }
}
