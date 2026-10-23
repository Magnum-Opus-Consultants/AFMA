namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.Data.Entities.Wims;
  using Alpha.Services.NHibernate.Repository.Services.Identity;
  using Microsoft.AspNetCore.Identity;

  /// <summary>
  /// The Identity Layer Interface.
  /// </summary>
  public interface IWimsIdentityLayer
  {
    /// <summary>
    /// Gets the <see cref="IdentityService{TIdentityUser}"/>.
    /// </summary>
    IIdentityService<WimsApplicationUser> Identity { get; }

    /// <summary>
    /// Returns a list of users with their associated roles.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>Returns a list of users in the specified role.</returns>
    Task<ICollection<UserModel>> GetUsersByRole(string roleName);

    /// <summary>
    /// Deletes an Identity User.
    /// </summary>
    /// <param name="userId">The user Id.</param>
    /// <returns>The result.</returns>
    Task<IdentityResult> DeleteUser(string userId);

    Task SendTestEmail();

    #region Login History

    Task LogLogin(string userId, string userName, string ipAddress, string hostName, string member, string memberBranch,
      string userType);

    #endregion
  }
}
