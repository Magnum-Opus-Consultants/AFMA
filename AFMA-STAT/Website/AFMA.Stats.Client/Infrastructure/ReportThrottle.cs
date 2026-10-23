namespace AFMA.Stats.Client.Infrastructure
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Caps concurrent heavy report (Excel) generations across the process so a burst of report
  /// requests cannot exhaust CPU/memory and take the whole site down for everyone else.
  /// Reports still run; they just queue briefly, and shed with a friendly message if saturated.
  /// </summary>
  public static class ReportThrottle
  {
    private static readonly SemaphoreSlim Gate = new SemaphoreSlim(Math.Max(2, Environment.ProcessorCount));

    public static Task<bool> WaitAsync() => Gate.WaitAsync(TimeSpan.FromSeconds(20));

    public static void Release() => Gate.Release();
  }
}
