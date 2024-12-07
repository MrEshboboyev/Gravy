using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;
using Gravy.Persistence.Users.Constants;

namespace Gravy.Persistence.Users.Configurations;

/// <summary> 
/// Configures the User entity for Entity Framework Core. 
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Map to the Users table
        builder.ToTable(UserTableNames.Users);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure relationships
        builder
            .HasOne(u => u.CustomerDetails)
            .WithOne()
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade deletion of Customer

        builder
            .HasOne(u => u.DeliveryPersonDetails)
            .WithOne()
            .HasForeignKey<DeliveryPerson>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade deletion of DeliveryPerson

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