﻿using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Persistence.Repositories;

public sealed class MenuItemRepository(ApplicationDbContext dbContext) : IMenuItemRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void Add(MenuItem menuItem) =>
        _dbContext.Set<MenuItem>().Add(menuItem);
}