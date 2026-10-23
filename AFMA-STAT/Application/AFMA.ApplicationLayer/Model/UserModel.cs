namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The UserModel class.
  /// </summary>
  public class UserModel
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserModel"/> class.
    /// </summary>
    public UserModel()
    {
      Roles = new List<RoleModel>();
    }

    /// <summary>
    /// The Id of the user.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The First Name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The Last Name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// The Full Name.
    /// </summary>
    public string? FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// The Email Address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The Last Login date.
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// The Receive Email indicator.
    /// </summary>
    public bool? ReceiveEmail { get; set; }

    /// <summary>
    /// The member name this user is linked to.
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// The member Id this user us linked to
    /// </summary>
    public int? MemberId { get; set; }

    /// <summary>
    /// The user roles.
    /// </summary>
    public ICollection<RoleModel>? Roles { get; set; }

    /// <summary>
    /// The user roles.
    /// </summary>
    public string? RoleNames { get; set; }

    public string? RoleNameFormatted
    {
      get
      {
        switch (RoleNames)
        {
          case "Admin":
            return "<span class='badge bg-info'>ADMIN</span>";

          case "User":
            return "<span class='badge bg-success'>USER</span>";

          default:
            return "<span class='badge bg-danger'>ERROR</span>";
        }
      }
    }
  }
}
