using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VkStorage.Application.Common.BaseRequests;
using VkStorage.Application.Common.Interfaces;
using VkStorage.Application.DTOs.VkFileDtos;
using VkStorage.Domain.Entities;

namespace VkStorage.Application.Services.Files;

public class UploadFileCommand : BaseRequest<UploadFileResponse>
{
    public required int userId { get; set; }
    public required IFormFile file { get; set; }
}

public class UploadFileResponse : BaseResponse
{
    public FilePreviewDto File { get; set; }
}

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.userId).NotEmpty().WithMessage("User ID is required");
        RuleFor(x => x.file).NotEmpty().WithMessage("File must be not empty");
    }
}

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, UploadFileResponse>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public UploadFileCommandHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UploadFileResponse> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        //string uploadUri = await GetUploadUri();
        //string uploadFileResponse = await UploadToVk(uploadUri, request.file);
        //dynamic response = await SaveChanges(uploadFileResponse);
        string url = "https://vk.com/doc-223718377_672740132?hash=5ZmLZ297QtGjt7R9ibUyKJHlGRmwvgF3NKs72oQd6rk&dl=ZesyfP3rSVO5Ifx97xBVVDl7DWAQZIH5pU1cXobh8pk&api=1&no_preview=1";
        await UpdateDbData(request.file.FileName, request.file.Length, request.userId, url);

        return new UploadFileResponse { File = new FilePreviewDto() { FileName = request.file.FileName, Guid = Guid.NewGuid(), SizeInBytes = request.file.Length, AccessLevel = Domain.Enums.AccessLevel.Strict } };
    }

    private async Task UpdateDbData(string fileName, long length, int userId, string url)
    {
        _context.VkFiles.Add(new VkFile(fileName, length, userId)
        {
            Chunks = new List<FileChunk>() { new FileChunk(url) },
        });
        await _context.SaveChangesAsync();
    }

    private async Task<string> GetUploadUri()
    {
        string fileUploadLinkUri = $"https://api.vk.com/method/docs.getWallUploadServer?group_id={Static.group_id}&access_token={Static.access_token}&v=5.199";
        using (var httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(fileUploadLinkUri);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).response.upload_url;
        }
    }

    private async Task<string> UploadToVk(string uploadUri, IFormFile file)
    {
        using (var httpClient = new HttpClient())
        {
            byte[] data;
            using (var br = new BinaryReader(file.OpenReadStream()))
                data = br.ReadBytes((int)file.OpenReadStream().Length);
            ByteArrayContent byteArray = new ByteArrayContent(data);
            string newFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName() + ".mp4");
            var content = new MultipartFormDataContent
            {
                { byteArray, "file", newFileName }
            };
            HttpResponseMessage response = await httpClient.PostAsync(uploadUri, content);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).file;
        }
    }

    private async Task<dynamic> SaveChanges(string uploadFileResponse)
    {
        using (var httpClient = new HttpClient())
        {
            string saveFileUri = "https://api.vk.com/method/docs.save";
            var content = new MultipartFormDataContent
            {
                { new StringContent(Static.access_token), "access_token"},
                { new StringContent(uploadFileResponse), "file" },
                { new StringContent("5.199"), "v" }
            };
            HttpResponseMessage response = await httpClient.PostAsync(saveFileUri, content);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        }
    }
}
