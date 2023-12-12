using VkStorage.Domain.Common;
using VkStorage.Domain.Enums;

namespace VkStorage.Domain.Entities;

public class VkFile : BaseClass
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public int UserId { get; set; }
    public long SizeInBytes { get; set; }
    public bool Deleted { get; set; } = false;
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
}
