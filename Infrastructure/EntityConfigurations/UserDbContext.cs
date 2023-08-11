using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityConfigurations;

public class UserDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "User";

    public DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }
}
