﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class DeleteComment
    {
        public record Command(string Slug, int CommentId) : IRequest<Result>;   

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Slug).NotEmpty();
                RuleFor(cmd => cmd.CommentId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceManager _serviceManager;
            private readonly ICurrentUserContext _currentUserContext;
            private readonly ILogger<Handler> _logger;

            public Handler(IServiceManager serviceManager, ICurrentUserContext currentUserContext, ILogger<DeleteComment.Handler> logger)
            {
                _serviceManager = serviceManager;
                _currentUserContext = currentUserContext;
                _logger = logger;
            }
            
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
               try
               {
                    var user = await _currentUserContext.GetCurrentUserContext();
                    var result = await _serviceManager.Articles.DeleteCommentAsync(request.Slug, request.CommentId, user.Id);
                    return Result.Success();
               }
               catch(Exception ex)
               {
                 _logger.LogError(ex, "Error occured while deleting comment");
                 return Result.Error(ex.Message);
               }
            }
        }
    }
}
