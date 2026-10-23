namespace AFMA.Data.Entities
{
  public class MaterialCategory
  {
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual MaterialGroup? MaterialGroup { get; set; }
    public virtual string? Code { get; set; }
    public virtual bool Active { get; set; }
  }
}
