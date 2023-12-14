using System.ComponentModel.DataAnnotations;
using VkStorage.Domain.Common;
using VkStorage.Domain.Enums;

namespace VkStorage.Domain.Entities;

public class VkFile : BaseClass
{
    [Required]
    public Guid Guid { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Extension { get; set; } = string.Empty;
    [Required]
    public int UserId { get; set; }
    [Required]
    public long SizeInBytes { get; set; }
    [Required]
    public bool Deleted { get; set; } = false;
    [Required]
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Strict;
    public IList<FileChunk>? Chunks { get; set; }

    public VkFile() { }

    public VkFile(string fileName, long sizeInBytes, int userId, AccessLevel accessLevel = AccessLevel.Strict)
    {
        Guid = Guid.NewGuid();
        Name = Path.GetFileNameWithoutExtension(fileName);
        Extension = Path.GetExtension(fileName);
        UserId = userId;
        SizeInBytes = sizeInBytes;
        AccessLevel = accessLevel;
    }

    public bool ValidateAccessLevel(int userId = 0)
    {
        if (AccessLevel == AccessLevel.Strict && userId != UserId)
            return false;
        if (AccessLevel == AccessLevel.Authorized && userId == 0)
            return false;
        return true;
    }
}
