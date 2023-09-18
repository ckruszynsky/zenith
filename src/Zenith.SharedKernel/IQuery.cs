using MediatR;

namespace Zenith.SharedKernel
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}
