using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWebApi.Domain.Constants;
using MyWebApi.Domain.Entities.Saga;

namespace MyWebApi.Infrastructure.Persistence.Mappings.Saga;

public sealed class MonitoringConfiguration : SagaClassMap<Monitoring>
{
    protected override void Configure(EntityTypeBuilder<Monitoring> entity, ModelBuilder model)
    {
        entity.ToTable("Monitoring", MyConstants.DefaultSchema);

        entity.HasKey(x => x.CorrelationId)
            .HasName("PK_Monitoring");

        entity
            .Property(e => e.CorrelationId)
            .HasColumnType("UniqueIdentifier")
            .IsRequired();

        entity
            .Property(x => x.CurrentState)
            .IsRequired();
        entity
            .Property(x => x.SubmittedAt);
        entity
            .Property(x => x.FinishedAt);

        base.Configure(entity, model);
    }
}
