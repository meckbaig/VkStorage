using MediatR;
using Newtonsoft.Json;
using VkStorage.Application.Common.BaseRequests;

namespace VkStorage.Application.Services.Authorization;

public class AuthorizeUserQuery : BaseRequest<AuthorizeUserResponse>
{
    public required string client_id { get; set; }
    public required string client_secret { get; set; }
    public required string redirect_uri { get; set; }
    public required string code { get; set; }
}

public class AuthorizeUserResponse : BaseResponse
{ 
    public dynamic User { get; set; }
}

public class AuthorizeUserQueryHandler : IRequestHandler<AuthorizeUserQuery, AuthorizeUserResponse>
{
    //private readonly IAppDbContext _context;
    //private readonly IMapper _mapper;

    public AuthorizeUserQueryHandler()//(IAppDbContext context, IMapper mapper)
    {
        //_context = context;
        //_mapper = mapper;
    }

    public async Task<AuthorizeUserResponse> Handle(AuthorizeUserQuery request, CancellationToken cancellationToken)
    {
        string token = await GetAccessTokenAsync(request);
        return await GetUserDataAsync(token);
    }

    private async Task<string> GetAccessTokenAsync(AuthorizeUserQuery request)
    {
        string accessTokenUri = $"https://oauth.vk.com/access_token?client_id={request.client_id}&client_secret={request.client_secret}&redirect_uri={request.redirect_uri}&code={request.code}";
        using (var httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(accessTokenUri);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).access_token;
        }
    }   

    private async Task<AuthorizeUserResponse> GetUserDataAsync(string accessToken)
    {
        using (var httpClient = new HttpClient())
        {
            string getUserUri = $"https://api.vk.com/method/account.getProfileInfo";
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "access_token", accessToken },
                { "v", "5.199" }
            };
            using (var content = new FormUrlEncodedContent(keyValuePairs))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.PostAsync(getUserUri, content);
                response.EnsureSuccessStatusCode();
                dynamic responseString = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).response;
                return new AuthorizeUserResponse() { User = responseString };
            }
        }
    }
}
