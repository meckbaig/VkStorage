using Microsoft.EntityFrameworkCore;
using VkStorage.Domain.Entities;
using VkStorage.Domain.Enums;

namespace VkStorage.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<VkFile> VkFiles { get; }
    DbSet<FileChunk> FileChunks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
