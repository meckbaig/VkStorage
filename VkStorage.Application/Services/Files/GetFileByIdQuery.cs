using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using VkStorage.Application.Common.BaseRequests;
using VkStorage.Application.Common.Interfaces;
using VkStorage.Application.DTOs.VkFileDtos;
using VkStorage.Domain.Entities;

namespace VkStorage.Application.Services.Files;

public class GetFileByIdQuery : BaseRequest<GetFileByIdResponse>
{
    public required string vkFileGuid { get; set; }
    public required int userId { get; set; }
}

public class GetFileByIdResponse : BaseResponse
{
    public required string FileType { get; set; }
    public required Stream ContentStream { get; set; }
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
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IGlobals _globals;

    public GetFileByIdQueryHandler(IAppDbContext context, IMapper mapper, IGlobals globals)
    {
        _context = context;
        _mapper = mapper;
        _globals = globals;
    }

    public async Task<GetFileByIdResponse> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        VkFile file = await _context.VkFiles.Include(f => f.Chunks).FirstOrDefaultAsync(f => Guid.Parse(request.vkFileGuid) == f.Guid);
        if (file == null)
            throw new ArgumentException("Не удалось найти запись");
        if (!file.ValidateAccessLevel(request.userId))
            throw new UnauthorizedAccessException("Нет доступа к записи");
        string downloadLink = await GetHtml(file?.Chunks[0]?.FileUrl);
        string filePath = await FetchFileFromLink(downloadLink, file.Name + file.Extension);
        return new GetFileByIdResponse() { FileType = GetFileType(file.Name + file.Extension), ContentStream = GetFileStream(filePath) };
    }

    private async Task<string> GetHtml(string fileUrl)
    {
        string html = "";
        Task downloadString = Task.Run(() =>
        {
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(fileUrl);
            }
        });
        string[] startSubstrings = ["docs_no_preview_download_btn_container", "href", "\""];
        string endSubstring = "\"";
        downloadString.Wait();
        return GetSubstring(html, startSubstrings, endSubstring);
    }

    private string GetSubstring(string original, string[] startStrings, string endString)
    {
        string result = original;
        foreach (string s in startStrings)
        {
            int index = result.IndexOf(s) + s.Length;
            result = result[index..result.Length];
        }
        return result[0..result.IndexOf(endString)].Replace("&amp;", "&");
    }

    private async Task<string> FetchFileFromLink(string downloadLink, string fileName)
    {
        using var client = new HttpClient();
        using var s = await client.GetStreamAsync(downloadLink);
        string filePath = Path.Combine(_globals.files_folder, fileName);
        using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
        await s.CopyToAsync(fs);
        return filePath;
    }

    private string GetFileType(string fileName)
    {
        string contentType;
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
        return contentType ?? "application/octet-stream";
    }

    private Stream GetFileStream(string filePath)
    {
        return new FileStreamDelete(filePath, FileMode.Open);
    }
}
