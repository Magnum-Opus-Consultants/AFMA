namespace AFMA.Web.Admin.Helpers
{
  public static class ViewHelpers
  {
    public static string ToBooleanActiveInactive(this bool value)
    {
      return value ? "active" : "";
    }

    public static string BooleanToYesNo(this bool value)
    {
      return value ? "Yes" : "No";
    }

    public static string BooleanToActiveInactive(this bool value)
    {
      return value ? "Active" : "Inactive";
    }
  }
}
