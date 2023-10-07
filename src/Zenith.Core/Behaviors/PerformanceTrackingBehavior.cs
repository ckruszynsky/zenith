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
        private readonly bool _isEnabled;
        private readonly int _elapsedMillisecondsThreshold;

        public PerformanceTrackingBehavior(
            ILogger<TRequest> logger,            
            IConfiguration configuration)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _isEnabled = Convert.ToBoolean(configuration["PipelineSettings:PerformanceTrackingEnabled"]);
            _elapsedMillisecondsThreshold = Convert.ToInt32(configuration["PipelineSettings:PerformanceTrackingThreshold"]);
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_isEnabled) 
            { 
                _timer.Start();
                    await next();
                _timer.Stop();

                var elapsedMilliseconds = _timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > _elapsedMillisecondsThreshold)
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
