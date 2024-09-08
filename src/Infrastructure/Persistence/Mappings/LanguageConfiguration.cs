using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWebApi.Domain.Entities;

namespace MyWebApi.Infrastructure.Persistence.Mappings
{
    internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Language", "dbo");

            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .HasMaxLength(2)
                .IsUnicode(false);

            builder.Property(e => e.CreatedDate)
                .HasColumnName("Created_Date")
                //.HasDefaultValueSql("CURRENT_TIMESTAMP");
                .HasDefaultValueSql("(getutcdate())");

            builder.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(128)
                .IsUnicode(false);

            builder.Property(e => e.LanguageId)
                .UseIdentityAlwaysColumn()
                .ValueGeneratedOnAdd();

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("Updated_Date");
        }
    }

}
