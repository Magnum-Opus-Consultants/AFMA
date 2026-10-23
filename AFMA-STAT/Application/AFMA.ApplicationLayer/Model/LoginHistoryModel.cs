namespace AFMA.ApplicationLayer.Model
{
  using System;

  /// <summary>
  /// The LoginHistoryModel Class
  /// </summary>
  [MapTo(typeof(LoginHistory))]
  public class LoginHistoryModel
  {
    /// <summary>
    /// Gets or sets the Record Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user Id.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the date of the login.
    /// </summary>
    public DateTime LoginDate { get; set; }

    /// <summary>
    /// Gets or sets the Ip address.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch.
    /// </summary>
    public string Memberbranch { get; set; }

    /// <summary>
    /// Gets or sets the Ip address.
    /// </summary>
    public string UserType { get; set; }

    /// <summary>
    /// Gets or sets the browser.
    /// </summary>
    public string Browser { get; set; }
  }
}
