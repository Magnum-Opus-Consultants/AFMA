namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class BatchMapping : Profile
  {
    public BatchMapping()
    {
      this.CreateMap<Batch, BatchModel>()
        .ForMember(d => d.DistributionGroupId, m => m.MapFrom(s => s.DistributionGroup.Id))
        .ForMember(d => d.DistributionGroup, m => m.MapFrom(s => s.DistributionGroup.Name))
        .ForMember(d => d.CommitteeId, m => m.MapFrom(s => s.Committee.Id))
        .ForMember(d => d.Committee, m => m.MapFrom(s => s.Committee.Name));

      this.CreateMap<BatchModel, Batch>()
        .ForMember(d => d.DistributionGroup, m => m.MapFrom(s => s.DistributionGroupId != null ? new DistributionGroup { Id = s.DistributionGroupId.Value } : null))
        .ForMember(d => d.Committee, m => m.MapFrom(s => s.CommitteeId != null ? new Committee { Id = s.CommitteeId.Value } : null));

      this.CreateMap<BatchAttachment, BatchAttachmentModel>()
        .ForMember(d => d.BatchId, m => m.MapFrom(s => s.Batch.Id));

      this.CreateMap<BatchAttachmentModel, BatchAttachment>()
        .ForMember(d => d.Batch, m => m.MapFrom(s => new Batch { Id = s.BatchId }));

      this.CreateMap<BatchRecipient, BatchRecipientModel>()
        .ForMember(d => d.BatchId, m => m.MapFrom(s => s.Batch.Id));

      this.CreateMap<BatchRecipientModel, BatchRecipient>()
        .ForMember(d => d.Batch, m => m.MapFrom(s => new Batch { Id = s.BatchId }));
    }
  }
}
