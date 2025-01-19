using MdConverter.DataAccess.Configurations;
using MdConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace MdConverter.DataAccess;

public class MdConverterDbContext : DbContext
{
    public MdConverterDbContext(DbContextOptions<MdConverterDbContext> options): base(options)
    {
    }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<DocumentEntity> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DocumentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}