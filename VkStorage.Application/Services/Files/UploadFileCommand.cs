using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
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
    private readonly IGlobals _statics;
    //private readonly ILogger _logger;

    public UploadFileCommandHandler(IAppDbContext context, IMapper mapper, IGlobals statics)// ,ILogger logger)
    {
        _context = context;
        _mapper = mapper;
        _statics = statics;
        //_logger = logger;
    }

    public async Task<UploadFileResponse> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        string filePath = "";
        Task getFilePath = Task.Run(() => { filePath = SaveFileToDrive(request.file); });
        string uploadUrl = await GetUploadUrl();
        getFilePath.Wait();
        string uploadFileResponse = string.IsNullOrEmpty(filePath) ? await UploadToVk(uploadUrl, request.file) : await UploadToVk(uploadUrl, filePath);
        Task.Run(async () => await DeleteTempFile(filePath));
        string docUrl = await SaveChanges(uploadFileResponse);
        //string url = "https://vk.com/doc-223718377_672740132?hash=5ZmLZ297QtGjt7R9ibUyKJHlGRmwvgF3NKs72oQd6rk&dl=ZesyfP3rSVO5Ifx97xBVVDl7DWAQZIH5pU1cXobh8pk&api=1&no_preview=1";
        VkFile vkFile = await UpdateDbData(request.file.FileName, request.file.Length, request.userId, docUrl);

        return new UploadFileResponse() { File = _mapper.Map<FilePreviewDto>(vkFile) };
    }

    private async Task DeleteTempFile(string filePath, int iteration = 1)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            if (++iteration> 5)
            {
                Console.WriteLine("Не удалось удалить временный файл : "+ ex.Message);
                //_logger.LogError(ex, "Не удалось удалить временный файл {@fileName}", Path.GetFileName(filePath));
                return;
            }
            await Task.Delay(10000);
            await DeleteTempFile(filePath, iteration);
        }
    }

    private string SaveFileToDrive(IFormFile file)
    {
        string path = Path.Combine(_statics.files_folder, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".mp4");
        if (!Path.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return path;
    }

    private async Task<VkFile> UpdateDbData(string fileName, long length, int userId, string url)
    {
        var vkFile = new VkFile(fileName, length, userId)
        {
            Chunks = new List<FileChunk>() { new FileChunk(url) },
        };
        _context.VkFiles.Add(vkFile);
        await _context.SaveChangesAsync();
        return vkFile;
    }

    private async Task<string> GetUploadUrl()
    {
        string fileUploadLinkUri = $"https://api.vk.com/method/docs.getWallUploadServer?group_id={_statics.group_id}&access_token={_statics.access_token}&v=5.199";
        using (var httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(fileUploadLinkUri);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).response.upload_url;
        }
    }

    //private async Task<string> UploadToVk(string uploadUri, string filePath)
    //{
    //    using (var httpClient = new HttpClient())
    //    {
    //        httpClient.Timeout = TimeSpan.FromMinutes(1000);
    //        using (var fileStream = File.OpenRead(filePath))
    //        {
    //            HttpResponseMessage response = await httpClient.PostAsync(uploadUri, new StreamContent(fileStream)); //content);
    //            response.EnsureSuccessStatusCode();
    //            string r = await response.Content.ReadAsStringAsync();
    //            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).file;
    //        }
    //    }
    //}

    private async Task<string> UploadToVk(string uploadUri, string filePath)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromMinutes(1000);
            using (var fileStream = File.OpenRead(filePath))
            {
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(fileStream), "file", Path.GetFileName(filePath) }
                };
                HttpResponseMessage response = await httpClient.PostAsync(uploadUri, content);
                response.EnsureSuccessStatusCode();
                string r = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).file;
            }
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
            string r = await response.Content.ReadAsStringAsync();
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
                { new StringContent(_statics.access_token), "access_token"},
                { new StringContent(uploadFileResponse), "file" },
                { new StringContent("5.199"), "v" }
            };
            HttpResponseMessage response = await httpClient.PostAsync(saveFileUri, content);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).response.doc.url;
        }
    }
}
