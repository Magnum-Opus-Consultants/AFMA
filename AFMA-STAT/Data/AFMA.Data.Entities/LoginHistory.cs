namespace AFMA.Data.Entities
{
  /// <summary>
  /// The LoginHistory Class
  /// </summary>
  public class LoginHistory
  {
    /// <summary>
    /// Gets or sets the Record Id.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the user Id.
    /// </summary>
    public virtual string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public virtual string UserName { get; set; }

    /// <summary>
    /// Gets or sets the date of the login.
    /// </summary>
    public virtual DateTime LoginDate { get; set; }

    /// <summary>
    /// Gets or sets the Ip address.
    /// </summary>
    public virtual string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    public virtual string HostName { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public virtual string Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch.
    /// </summary>
    public virtual string Memberbranch { get; set; }

    /// <summary>
    /// Gets or sets the Ip address.
    /// </summary>
    public virtual string UserType { get; set; }

    /// <summary>
    /// Gets or sets the browser.
    /// </summary>
    public virtual string Browser { get; set; }
  }
}