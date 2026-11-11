using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Core.Outbox.Storage;

public class OutboxEvent : IPortalEntity
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? JsonData { get; set; }
    public string? EventType { get; set; }
    public bool WasSent { get; set; }
}

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    private readonly string _tableName;
    private readonly string _schema;

    public OutboxEventConfiguration(string tableName, string schema)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);

        _tableName = tableName;
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(name: _tableName, schema: _schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.CreatedAtUtc);

        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.JsonData)
            .HasMaxLength(8192)
            .IsRequired();

        builder.Property(x => x.EventType).IsRequired();

        builder.Property(x => x.WasSent).IsRequired();
        builder.HasIndex(x => x.WasSent);
    }
}
