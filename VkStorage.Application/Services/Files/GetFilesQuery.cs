using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VkStorage.Application.Common.BaseRequests;
using VkStorage.Application.Common.Interfaces;
using VkStorage.Application.DTOs.VkFileDtos;
using VkStorage.Domain.Entities;
using VkStorage.Domain.Enums;

namespace VkStorage.Application.Services.Files;

public class GetFilesQuery : BaseRequest<GetFilesResponse>
{
    public int userId { get; set; }
}

public class GetFilesResponse : BaseResponse
{
    public List<FilePreviewDto> Files { get; set; }
}

public class GetFilesQueryValidator : AbstractValidator<GetFilesQuery>
{
    public GetFilesQueryValidator()
    {
        RuleFor(x => x.userId).NotEmpty().WithMessage("User ID is required");
    }
}

public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, GetFilesResponse>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetFilesQueryHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetFilesResponse> Handle(GetFilesQuery request, CancellationToken cancellationToken)
    {
        var files = await _context.VkFiles.Where(vkf => vkf.UserId == request.userId && !vkf.Deleted)
            .OrderByDescending(vkf => vkf.Name).ProjectTo<FilePreviewDto>(_mapper.ConfigurationProvider).ToListAsync();
        return new GetFilesResponse { Files = files };
    }
}
