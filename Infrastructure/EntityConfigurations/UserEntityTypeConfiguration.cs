using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(userId => userId.ToString(),
                value => Guid.Parse(value));

        builder.OwnsOne(x => x.Email, 
            options =>
            {
                options
                    .HasIndex(e => e.Value)
                    .IsUnique()
                    .HasDatabaseName("Unique_User_Email");
            });
    }
}
