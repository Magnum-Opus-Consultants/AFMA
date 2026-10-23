namespace AFMA.Data.Entities.Wims
{
  using NHibernate.AspNetCore.Identity;
  using System.Text;

  /// <summary>
  /// The Application User Entity Class.
  /// </summary>
  public class WimsApplicationUser : IdentityUser
  {
    /// <summary>
    /// Gets or Sets the First Name.
    /// </summary>
    public virtual string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the Last Name.
    /// </summary>
    public virtual string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a the user Created Date.
    /// </summary>
    public virtual DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the Last Login Date.
    /// </summary>
    public virtual DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not a user is active.
    /// </summary>
    public virtual bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not a user received an email.
    /// </summary>
    public virtual bool ReceivedEmail { get; set; }

    /// <summary>
    /// Gets or sets the full name of the member account.
    /// </summary>
    public virtual string? FullName
    {
      get
      {
        if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)) return string.Empty;
        var fullName = new StringBuilder();
        fullName.Append(this.FirstName);
        fullName.Append(" ");
        fullName.Append(this.LastName);
        return fullName.ToString();
      }
    }
  }
}
