using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DomainEventLogConfiguration : IEntityTypeConfiguration<DomainEventLog>
{
    public void Configure(EntityTypeBuilder<DomainEventLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.EventData)
            .IsRequired();

        builder.Property(e => e.OccurredOn)
            .IsRequired();

        builder.Property(e => e.AggregateId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.AggregateName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Processed)
            .IsRequired();

        builder.Property(e => e.ProcessedOn);

        // Índices para melhorar a performance de consultas comuns
        builder.HasIndex(e => e.AggregateId);
        builder.HasIndex(e => e.Processed);
        builder.HasIndex(e => e.EventType);
    }
}