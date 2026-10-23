namespace Resolvetech.NetCore.Common.Helpers
{
  /// <summary>
  /// Local stand-in for the Resolvetech.NetCore.Common package, which is not available
  /// on the local NuGet feed. Only the members the solution actually consumes are
  /// implemented; the badge markup follows the conventions of the surrounding views
  /// (see <c>AFMA.ApplicationLayer.Helpers.GeneralHelpers.ToFormattedMemberStatus</c>)
  /// rather than being copied from the real package.
  /// </summary>
  public static class FormattingHelper
  {
    /// <summary>Renders a boolean as a YES/NO badge.</summary>
    public static string ToFormattedYesNo(this bool value)
    {
      if (!value)
      {
        return "<span class=\"badge bg-secondary\">NO</span>";
      }

      return "<span class=\"badge bg-success\">YES</span>";
    }

    /// <summary>Renders a boolean as an ACTIVE/INACTIVE badge.</summary>
    public static string ToFormattedActiveInactive(this bool value)
    {
      if (!value)
      {
        return "<span class=\"badge bg-danger\">INACTIVE</span>";
      }

      return "<span class=\"badge bg-success\">ACTIVE</span>";
    }
  }
}
