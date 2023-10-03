using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Users
{
    public class LoginUser
    {
        public record Command(string Email, string Password) : IRequest<Result<UserViewModel>>;

        public class Validator : AbstractValidator<LoginUser.Command>
        {
            public Validator()
            {
                RuleFor(c => c.Email).EmailAddress().NotEmpty();
                RuleFor(c => c.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<LoginUser.Command, Result<UserViewModel>>
        {
            private readonly UserManager<ZenithUser> _userManager;
            private readonly ITokenService _tokenService;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(
                UserManager<ZenithUser> userManager,
                ITokenService tokenService,
                ILogger<LoginUser.Handler> logger,
                IMapper mapper,
                IMediator mediator)
            {
                _userManager = userManager;                
                _tokenService = tokenService;
                _logger = logger;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Result<UserViewModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email.ToUpperInvariant());
                if (existingUser == null)
                {
                    return Result.Error($"Incorrect user or password");
                }

                var existingUserPasswordMatch = await _userManager.CheckPasswordAsync(existingUser, request.Password);
                if (!existingUserPasswordMatch)
                {
                    return Result.Error("Incorrect user or password");
                }

                var token = _tokenService.CreateToken(existingUser);
                var user = _mapper.Map<UserViewModel>(existingUser);
                user.Token = token;

                _logger.LogInformation($"Login successful for user [{existingUser.Id} ({existingUser.Email}]");

                await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                {
                    ActivityType = ActivityType.Login,
                    TransactionType = TransactionType.ZenithUser,
                    TransactionId = existingUser.Id
                }), cancellationToken);
                return Result.Success(user);
            }
        }



    }
}
