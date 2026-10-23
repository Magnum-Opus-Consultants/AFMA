namespace AFMA.Data.Entities
{
  public class Product
  {
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual ProductGroup? ProductGroup { get; set; }
    public virtual string? Code { get; set; }
    public virtual int DisplayOrder { get; set; }
    public virtual bool Active { get; set; }
    public virtual bool VisibleToMembers { get; set; }
    public virtual bool Other { get; set; }
    public virtual bool AllowDuplicates { get; set; }
  }
}
