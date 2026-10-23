namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The MemberContact entity class represents a contact person for a member in the WIMS application.
  /// </summary>
  public class MemberBranchContact
  {
    /// <summary>
    /// The unique identifier for the Province.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the Province.
    /// </summary>
    public virtual Member? Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    public virtual MemberBranch? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the member number contact type.
    /// </summary>
    public virtual MemberContactType MemberContactType { get; set; } = new();

    /// <summary>
    /// Gets or sets the title of the contact person.
    /// </summary>
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the contact person.
    /// </summary>
    public virtual string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the surname of the contact person.
    /// </summary>
    public virtual string Surname { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the telephone number of the contact person.
    /// </summary>
    public virtual string TelephoneNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mobile number of the contact person.
    /// </summary>
    public virtual string MobileNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the contact person.
    /// </summary>
    public virtual string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company-wide indicator
    /// </summary>
    public virtual bool CompanyContact { get; set; }
  }
}
