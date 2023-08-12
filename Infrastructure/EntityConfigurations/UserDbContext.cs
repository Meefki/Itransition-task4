using Domain;
using Microsoft.EntityFrameworkCore;
using System;

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

        modelBuilder.Entity<User>().HasData(new List<User>()
        {
            new(Guid.NewGuid(), "Amie Arroyo", "123", new("AmieArroyo@mail.com")),
            new(Guid.NewGuid(), "Millicent Barr", "123", new("MillicentBarr@mail.com")),
            new(Guid.NewGuid(), "Abigail Cantu", "123", new("AbigailCantu@mail.com")),
            new(Guid.NewGuid(), "Cora Mitchell", "123", new("CoraMitchell@mail.com")),
            new(Guid.NewGuid(), "Cara Mcgowan", "123", new("CaraMcgowan@mail.com")),
            new(Guid.NewGuid(), "Alesha Harding", "123", new("AleshaHarding@mail.com")),
            new(Guid.NewGuid(), "Kyla Mccullough", "123", new("KylaMccullough@mail.com")),
            new(Guid.NewGuid(), "Christine Espinoza", "123", new("ChristineEspinoza@mail.com")),
            new(Guid.NewGuid(), "Nina Hernandez", "123", new("NinaHernandez@mail.com")),
            new(Guid.NewGuid(), "Alexia Matthews", "123", new("AlexiaMatthews@mail.com")),

            new(Guid.NewGuid(), "Troy Larson", "123", new("TroyLarson@mail.com")),
            new(Guid.NewGuid(), "Elise Hogan", "123", new("EliseHogan@mail.com")),
            new(Guid.NewGuid(), "Scarlet Gonzalez", "123", new("ScarletGonzalez@mail.com")),
            new(Guid.NewGuid(), "Henry Lindsey", "123", new("HenryLindsey@mail.com")),
            new(Guid.NewGuid(), "Hassan Small", "123", new("HassanSmall@mail.com")),
            new(Guid.NewGuid(), "Marco Patton", "123", new("MarcoPatton@mail.com")),
            new(Guid.NewGuid(), "Louis Hayden", "123", new("LouisHayden@mail.com")),
            new(Guid.NewGuid(), "Carlos Holder", "123", new("CarlosHolder@mail.com")),
            new(Guid.NewGuid(), "Arran Solis", "123", new("ArranSolis@mail.com")),
            new(Guid.NewGuid(), "Thomas Mcconnell", "123", new("ThomasMcconnell@mail.com")),

            new(Guid.NewGuid(), "Phillip Mata", "123", new("PhillipMata@mail.com")),
            new(Guid.NewGuid(), "Conner Clarke", "123", new("ConnerClarke@mail.com")),
            new(Guid.NewGuid(), "Zachary David", "123", new("ZacharyDavid@mail.com")),
            new(Guid.NewGuid(), "Amir Meyer", "123", new("AmirMeyer@mail.com")),
            new(Guid.NewGuid(), "admin", "123", new("admin@mail.com")),
            new(Guid.NewGuid(), "vitalii", "123", new("vitalii@mail.com")),
        });
    }
}
