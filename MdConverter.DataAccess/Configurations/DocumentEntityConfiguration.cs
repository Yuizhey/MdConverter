using MdConverter.Core.Models;
using MdConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MdConverter.DataAccess.Configurations;

public class DocumentEntityConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).IsRequired();
        builder.Property(i => i.Name).IsRequired().HasMaxLength(Document.MAX_NAME_LENGTH);
        builder.Property(i=> i.UsertId).IsRequired();
    }
}