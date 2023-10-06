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

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if(!_isEnabled) return;
            var requestName = typeof(TRequest).Name;
            var user = await _currentUserContext.GetCurrentUserContext();            
            var userId = user.Id ?? string.Empty;
            var userName = user.UserName ?? string.Empty;
            _logger.LogInformation("Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);
        }
    }
}
