using Gravy.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;

namespace Gravy.Persistence.Configurations;

/// <summary> 
/// Configures the User entity for Entity Framework Core. 
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Map to the Users table
        builder.ToTable(TableNames.Users);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure property conversions and constraints
        builder
            .Property(x => x.Email)
            .HasConversion(x => x.Value, v => Email.Create(v).Value);
        builder
            .Property(x => x.FirstName)
            .HasConversion(x => x.Value, v => FirstName.Create(v).Value)
            .HasMaxLength(FirstName.MaxLength);
        builder
            .Property(x => x.LastName)
            .HasConversion(x => x.Value, v => LastName.Create(v).Value)
            .HasMaxLength(LastName.MaxLength);

        // Configure unique constraint on Email
        builder.HasIndex(x => x.Email).IsUnique();
    }
}