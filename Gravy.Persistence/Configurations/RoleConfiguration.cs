﻿using Gravy.Domain.Entities;
using Gravy.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gravy.Persistence.Configurations;

/// <summary> 
/// Configures the Role entity for Entity Framework Core. 
/// </summary>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Map to the Roles table
        builder.ToTable(TableNames.Roles);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure many-to-many relationships
        builder.HasMany(x => x.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();
        
        builder.HasMany(x => x.Users)
            .WithMany(x => x.Roles);

        // Seed initial data
        builder.HasData(Role.GetValues());
    }
}