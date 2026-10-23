namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class NotificationMapping : Profile
  {
    public NotificationMapping()
    {
      this.CreateMap<NotificationTemplate, NotificationTemplateModel>()
        .ForMember(d => d.NotificationTypeId, m => m.MapFrom(s => s.NotificationType.Id))
        .ForMember(d => d.NotificationType, m => m.MapFrom(s => s.NotificationType.Description));

      this.CreateMap<NotificationTemplateModel, NotificationTemplate>()
        .ForMember(d => d.NotificationType, m => m.MapFrom(s => new NotificationType { Id = s.NotificationTypeId }));

      this.CreateMap<NotificationContact, NotificationContactModel>()
        .ForMember(d => d.NotificationTemplateId, m => m.MapFrom(s => s.NotificationTemplate.Id))
        .ForMember(d => d.NotificationTemplate, m => m.MapFrom(s => s.NotificationTemplate.Description))
        .ForMember(d => d.MemberContactTypeId, m => m.MapFrom(s => s.MemberContactType.Id))
        .ForMember(d => d.MemberContactType, m => m.MapFrom(s => s.MemberContactType.Description));

      this.CreateMap<NotificationContactModel, NotificationContact>()
        .ForMember(d => d.NotificationTemplate, m => m.MapFrom(s => new NotificationTemplate { Id = s.NotificationTemplateId }))
        .ForMember(d => d.MemberContactType, m => m.MapFrom(s => s.MemberContactTypeId != null ? new MemberContactType { Id = s.MemberContactTypeId.Value } : null));
    }
  }
}
