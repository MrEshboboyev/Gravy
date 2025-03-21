﻿using Gravy.Domain.Entities;
using Gravy.Persistence.Users.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gravy.Persistence.Users.Configurations.Roles.Permissions;

/// <summary> 
/// Configures the Permission entity for Entity Framework Core. 
/// </summary>
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Map to the Permissions table
        builder.ToTable(UserTableNames.Permissions);

        // Configure the primary key
        builder.HasKey(p => p.Id);

        // Seed initial data
        IEnumerable<Permission> permissions = Enum
            .GetValues<Domain.Enums.Permission>()
            .Select(p => new Permission
            {
                Id = (int)p,
                Name = p.ToString(),
            });

        builder.HasData(permissions);
    }
}
