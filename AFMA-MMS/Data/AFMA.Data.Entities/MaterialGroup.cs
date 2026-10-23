namespace AFMA.Data.Entities
{
  public class MaterialGroup
  {
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Code { get; set; }
    public virtual bool Active { get; set; }
  }
}