using MediatR;

namespace Zenith.SharedKernel
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
