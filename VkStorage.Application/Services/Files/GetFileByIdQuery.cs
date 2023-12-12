using AutoMapper;
using FluentValidation;
using MediatR;
using VkStorage.Application.Common.BaseRequests;

namespace VkStorage.Application.Services.Files;

public class GetFileByIdQuery : BaseRequest<GetFileByIdResponse>
{
    public required string vkFileGuid { get; set; }
    public required int userId { get; set; }
}

public class GetFileByIdResponse : BaseResponse
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
}

public class GetFileByIdQueryValidator : AbstractValidator<GetFileByIdQuery>
{
    public GetFileByIdQueryValidator()
    {
        RuleFor(x => x.vkFileGuid).Must(CanParseToGuid).WithMessage("VkFile Guid must be valid Guid");
        RuleFor(x => x.userId).NotEmpty().WithMessage("User ID is required");
    }

    private bool CanParseToGuid(string value)
    {
        Guid tmpGuid;
        return Guid.TryParse(value, out tmpGuid);
    }
}

public class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, GetFileByIdResponse>
{
    //private readonly IAppDbContext _context;
    //private readonly IMapper _mapper;

    public GetFileByIdQueryHandler()//(IAppDbContext context, IMapper mapper)
    {
        //_context = context;
        //_mapper = mapper;
    }

    public async Task<GetFileByIdResponse> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        /// TODO: написать парсер страницы для загрузки, понять, как файл передавать на клиент для корректного скачивания.
        throw new NotImplementedException();
    }
}
