using System.Security.Claims;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Behaviors
{
    public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly bool _isEnabled;
        
        public LoggingBehavior(ILogger<TRequest> logger, ICurrentUserContext currentUserContext, IConfiguration configuration)
        {
            _logger = logger;
            _currentUserContext = currentUserContext;
            _isEnabled = Convert.ToBoolean(configuration["PipelineSettings:LoggingEnabled"]);
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if(!_isEnabled) return Task.CompletedTask;
            var requestName = typeof(TRequest).Name;
            var user = _currentUserContext.GetCurrentUserContext();                        
            _logger.LogInformation("Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, user.Id, user.UserName, request);
            return Task.CompletedTask;
        }
    }
}
