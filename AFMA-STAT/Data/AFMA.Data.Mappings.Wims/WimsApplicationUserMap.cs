namespace AFMA.Data.Mappings
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// This class represents the AspNetUser Table Mappings.
  /// </summary>
  public class WimsApplicationUserMap : ClassMap<WimsApplicationUser>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="WimsApplicationUserMap"/> class.
    /// </summary>
    public WimsApplicationUserMap()
    {
      // Default AspNet Data
      this.Table("AspNetUsers");
      this.Id(x => x.Id).Not.Nullable().GeneratedBy.UuidHex("D");
      this.Map(x => x.AccessFailedCount).Not.Nullable().Default("0");
      this.Map(x => x.ConcurrencyStamp).Nullable();
      this.Map(x => x.Email).Nullable();
      this.Map(x => x.EmailConfirmed).Not.Nullable();
      this.Map(x => x.LockoutEnabled).Not.Nullable();
      this.Map(x => x.LockoutEnd).Nullable();
      this.Map(x => x.NormalizedEmail).Nullable();
      this.Map(x => x.NormalizedUserName).Nullable();
      this.Map(x => x.PasswordHash).Nullable();
      this.Map(x => x.PhoneNumber).Nullable();
      this.Map(x => x.PhoneNumberConfirmed).Not.Nullable();
      this.Map(x => x.SecurityStamp).Nullable();
      this.Map(x => x.TwoFactorEnabled).Not.Nullable();
      this.Map(x => x.UserName).Not.Nullable();

      // Additional Data
      this.Map(x => x.FirstName).Not.Nullable();
      this.Map(x => x.LastName).Not.Nullable();
      this.Map(x => x.CreatedDate).Not.Nullable();
      this.Map(x => x.LastLoginDate).Nullable();
      this.Map(x => x.IsActive).Not.Nullable();
      this.Map(x => x.ReceivedEmail).Not.Nullable();
    }
  }
}