using VkStorage.Application.Services.Authorization;

namespace VkStorage.Web.Controllers
{
    [ApiController]
    [Route("api/v{version:ApiVersion}/[controller]")]
    [ApiVersion("1")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorizationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //public override void Map(WebApplication app)
        //{
        //    app.MapGroup(this)
        //        .MapGet(AuthorizeUser);
        //}


        [HttpGet]
        //[Route("AuthorizeUser")]
        public async Task<ActionResult<AuthorizeUserResponse>> AuthorizeUser([FromQuery] AuthorizeUserQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToJsonResponseAsync();
        }
    }
}
