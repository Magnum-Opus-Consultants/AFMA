namespace AFMA.Data.Entities
{
  using System;

  /// <summary>
  /// The MemberBranch entity class.
  /// </summary>
  public class MemberBranch
  {
    /// <summary>
    /// Gets or sets the Id of the branch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the branch.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the code of the branch.
    /// </summary>
    public virtual string? Code { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public virtual Member? Member { get; set; }

    /// <summary>
    /// Gets or sets the province of the branch.
    /// </summary>
    public virtual Province? Province { get; set; }

    /// <summary>
    /// Gets or sets the status of the branch.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the created date of the branch.
    /// </summary>
    public virtual DateTime CreatedDate { get; set; }
  }

  /// <summary>
  /// The MemberBranchSummary entity class.
  /// </summary>
  public class MemberBranchSummary
  {
    /// <summary>
    /// Gets or sets the Id of the branch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the branch.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the code of the branch.
    /// </summary>
    public virtual string? Code { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public virtual MemberSummary? Member { get; set; }

    /// <summary>
    /// Gets or sets the province of the branch.
    /// </summary>
    public virtual Province? Province { get; set; }

    /// <summary>
    /// Gets or sets the status of the branch.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}