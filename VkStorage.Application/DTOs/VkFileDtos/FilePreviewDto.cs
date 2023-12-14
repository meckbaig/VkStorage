using AutoMapper;
using VkStorage.Domain.Entities;
using VkStorage.Domain.Enums;

namespace VkStorage.Application.DTOs.VkFileDtos
{
    public record FilePreviewDto
    {
        public Guid Guid { get; set; }
        public string FileName { get; set; }
        public long SizeInBytes { get; set; }
        public AccessLevel AccessLevel { get; set; }

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<VkFile, FilePreviewDto>()
                    .ForMember(dto => dto.FileName, 
                    opt => opt.MapFrom(o => o.Name + o.Extension));
            }
        }
    }
}
