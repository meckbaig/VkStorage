using MediatR;

namespace VkStorage.Application.Common.BaseRequests
{
    public class BaseRequest<TResponse> : IRequest<TResponse> where TResponse : BaseResponse { }
}
