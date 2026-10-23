namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class DocumentMapping : Profile
  {
    public DocumentMapping()
    {
      this.CreateMap<DocumentTemplate, DocumentTemplateModel>().ReverseMap();

      this.CreateMap<DocumentType, DocumentTypeModel>()
        .ForMember(d => d.DocumentTemplateId, m => m.MapFrom(s => s.DocumentTemplate.Id))
        .ForMember(d => d.DocumentTemplate, m => m.MapFrom(s => s.DocumentTemplate.Name));

      this.CreateMap<DocumentTypeModel, DocumentType>()
        .ForMember(d => d.DocumentTemplate, m => m.MapFrom(s => s.DocumentTemplateId != null ? new DocumentTemplate { Id = s.DocumentTemplateId.Value } : null));

      this.CreateMap<MemberDocument, MemberDocumentModel>()
        .ForMember(d => d.DocumentTypeId, m => m.MapFrom(s => s.DocumentType.Id))
        .ForMember(d => d.DocumentType, m => m.MapFrom(s => s.DocumentType.Description))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.MemberTradeName));

      this.CreateMap<MemberDocumentModel, MemberDocument>()
        .ForMember(d => d.DocumentType, m => m.MapFrom(s => new DocumentType { Id = s.DocumentTypeId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }));
    }
  }
}
