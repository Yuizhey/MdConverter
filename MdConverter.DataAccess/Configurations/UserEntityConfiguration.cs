using MdConverter.Core.Models;
using MdConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MdConverter.DataAccess.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).IsRequired();
        builder.Property(i => i.Name).IsRequired().HasMaxLength(User.MAX_PARAMETER_LENGTH);
        builder.Property(i => i.PasswordHash).IsRequired();
    }
}