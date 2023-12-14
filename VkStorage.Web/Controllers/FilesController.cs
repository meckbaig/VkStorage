using VkStorage.Application.Services.Files;

namespace VkStorage.Web.Controllers
{
    [ApiController]
    [Route("api/v{version:ApiVersion}/[controller]")]
    [ApiVersion("1")]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Сервер активен");
        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult<GetFilesResponse>> GetFiles([FromQuery] GetFilesQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToJsonResponseAsync();
        }

        [HttpGet]
        [Route("Get/{vkFileGuid}")]
        public async Task<ActionResult<GetFileByIdResponse>> GetFileById(string vkFileGuid, [FromQuery] int userId)
        {
            var query = new GetFileByIdQuery() { userId = userId, vkFileGuid = vkFileGuid };
            var result = await _mediator.Send(query);
            if (result.GetException() != null)
                throw result.GetException() ?? new Exception(result.Message);
            return new FileStreamResult(result.ContentStream, result.FileType);
        }

        [HttpPost]
        [Route("Upload")]
        [RequestSizeLimit(2_100_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 2_100_000_000)]
        public async Task<ActionResult<UploadFileResponse>> UploadFile([FromForm] UploadFileCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToJsonResponseAsync();
        }
    }
}
