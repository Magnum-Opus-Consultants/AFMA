namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Product Group Class.
  /// </summary>
  public class ProductGroup
  {
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Code { get; set; }
    public virtual int DisplayOrder { get; set; }
    public virtual bool Active { get; set; }
    public virtual bool HideOnFeedsales { get; set; }
    public virtual bool HideOnSalmonella { get; set; }
    public virtual bool VisibleToMembers { get; set; }
    public virtual SalmonellaProductGroupCategory? SalmonellaProductGroupCategory { get; set; }
    public virtual ICollection<Product> Products { get; set; }
  }
}
