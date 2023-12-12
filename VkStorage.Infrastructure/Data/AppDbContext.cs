using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using VkStorage.Application.Common.Interfaces;
using VkStorage.Domain.Common;
using VkStorage.Domain.Entities;
using VkStorage.Domain.Enums;

namespace VkStorageSQlite.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<VkFile> VkFiles => Set<VkFile>();
    public DbSet<FileChunk> FileChunks => Set<FileChunk>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<VkFile>(e => e.HasIndex(e => e.Id).IsUnique());
        builder.Entity<FileChunk>(e => e.HasIndex(e => e.Id).IsUnique());

        base.OnModelCreating(builder);
    }
}
