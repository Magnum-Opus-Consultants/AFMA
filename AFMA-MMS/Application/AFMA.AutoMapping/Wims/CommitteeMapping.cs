namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class CommitteeMapping : Profile
  {
    public CommitteeMapping()
    {
      this.CreateMap<Committee, CommitteeModel>().ReverseMap();

      this.CreateMap<CommitteeDocument, CommitteeDocumentModel>()
        .ForMember(d => d.DocumentTypeId, m => m.MapFrom(s => s.DocumentType.Id))
        .ForMember(d => d.DocumentType, m => m.MapFrom(s => s.DocumentType.Description))
        .ForMember(d => d.CommitteeId, m => m.MapFrom(s => s.Committee.Id))
        .ForMember(d => d.Committee, m => m.MapFrom(s => s.Committee.Name));

      this.CreateMap<CommitteeDocumentModel, CommitteeDocument>()
        .ForMember(d => d.DocumentType, m => m.MapFrom(s => new DocumentType { Id = s.DocumentTypeId }))
        .ForMember(d => d.Committee, m => m.MapFrom(s => new Committee { Id = s.CommitteeId }));

      this.CreateMap<MemberCommittee, MemberCommitteeModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.MemberLegalName))
        .ForMember(d => d.MemberName, m => m.MapFrom(s => s.Member.MemberLegalName))
        .ForMember(d => d.MemberBranchContactId, m => m.MapFrom(s => s.MemberBranchContact.Id))
        .ForMember(d => d.MemberBranchContact, m => m.MapFrom(s => $"{s.MemberBranchContact.FirstName} {s.MemberBranchContact.Surname}"))
        .ForMember(d => d.CommitteeId, m => m.MapFrom(s => s.Committee.Id))
        .ForMember(d => d.Committee, m => m.MapFrom(s => s.Committee.Name))
        .ForMember(d => d.Documents, m => m.MapFrom(s => s.Committee.Documents));

      this.CreateMap<MemberCommitteeModel, MemberCommittee>()
        .ForMember(d => d.Member, m => m.MapFrom(s => s.MemberId != null ? new Member { Id = s.MemberId.Value } : null))
        .ForMember(d => d.MemberBranchContact, m => m.MapFrom(s => s.MemberBranchContactId != null ? new MemberBranchContact { Id = s.MemberBranchContactId.Value } : null))
        .ForMember(d => d.Committee, m => m.MapFrom(s => new Committee { Id = s.CommitteeId }));
    }
  }
}
