using System.ComponentModel.DataAnnotations.Schema;
using VkStorage.Domain.Common;

namespace VkStorage.Domain.Entities;

public class FileChunk : BaseClass
{
    public string? FileUrl { get; set; }
    [ForeignKey(nameof(VkFileId))]
    public int VkFileId { get; set; }
    public VkFile VkFile { get; set; }

    public FileChunk() { }
    public FileChunk(int fileId)
    {
        VkFileId = fileId;
    }
    public FileChunk(string fileUrl)
    {
        FileUrl = fileUrl;
    }
}
