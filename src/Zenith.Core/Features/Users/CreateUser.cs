using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Users.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Users
{
    public class CreateUser
    {
        public record Command(CreateUserDto CreateUserDto) : IRequest<Result<UserViewModel>>;
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(u => u.CreateUserDto.Email)
                    .NotEmpty()
                    .EmailAddress();

                RuleFor(u => u.CreateUserDto.Username)
                    .NotEmpty();

                RuleFor(u => u.CreateUserDto.Password)
                    .NotEmpty();
            }
        }
        public class Handler : IRequestHandler<CreateUser.Command, Result<UserViewModel>>
        {
            private readonly UserManager<ZenithUser> _userManager;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly ITokenService _tokenService;
            private readonly IMediator _mediator;

            public Handler(
                ILogger<Handler> logger,
                UserManager<ZenithUser> userManager,
                IMapper mapper,
                ITokenService tokenService,
                IMediator mediator
                )
            {
                _logger = logger;
                _userManager = userManager;
                _mapper = mapper;
                _tokenService = tokenService;
                _mediator = mediator;
            }
            public async Task<Result<UserViewModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                Guard.Against.Null(request, nameof(request));

                var existingUserByUserName = await _userManager.FindByNameAsync(request.CreateUserDto.Username);
                if (existingUserByUserName != null)
                {
                    return Result.Error($"Username {request.CreateUserDto.Username} is already in use");
                }

                var existingUserByEmail = await _userManager.FindByEmailAsync(request.CreateUserDto.Email);
                if (existingUserByEmail != null)
                {
                    return Result.Error($"Email {request.CreateUserDto.Email} is already in use");
                }

                var newUser = new ZenithUser
                {
                    UserName = request.CreateUserDto.Username,
                    Email = request.CreateUserDto.Email,
                };

                var createUserResult = await _userManager.CreateAsync(newUser, request.CreateUserDto.Password);

                if (!createUserResult.Succeeded)
                {
                    var errors = new List<string>();
                    foreach (var error in createUserResult.Errors)
                    {
                        errors.Add($"{error.Code} - {error.Description}");
                    }
                    return Result.Error(errors.ToArray());
                }

                var token = _tokenService.CreateToken(newUser);

                var userViewModel = _mapper.Map<UserViewModel>(newUser);
                userViewModel.Token = token;

                await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                {
                    ActivityType = ActivityType.UserCreated,
                    TransactionType = TransactionType.ZenithUser,
                    TransactionId = newUser.Id
                }), cancellationToken);
                return Result.Success(userViewModel);
            }
        }
    }
}
