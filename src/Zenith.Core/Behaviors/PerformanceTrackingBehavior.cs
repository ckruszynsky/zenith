using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Behaviors
{
    public class PerformanceTrackingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserContext _currentUserContext;        
        private readonly bool _isEnabled;

        public PerformanceTrackingBehavior(
            ILogger<TRequest> logger,
            ICurrentUserContext currentUserService,
            IConfiguration configuration)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _isEnabled = Convert.ToBoolean(configuration["PipelineSettings:PerformanceTrackingBehavior"]);
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_isEnabled) 
            { 
                _timer.Start();
                var response = await next();
                _timer.Stop();

                var elapsedMilliseconds = _timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > 500)
                {
                    var requestName = typeof(TRequest).Name;               
                    
                        _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                        requestName, elapsedMilliseconds,request);
                }

            }

            return await next();
        }

    }
}
