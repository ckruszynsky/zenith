using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
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
            private readonly AppDbContext _appDbContext;
            private readonly ITokenService _tokenService;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(
                UserManager<ZenithUser> userManager,
                AppDbContext appDbContext,
                ITokenService tokenService,
                ILogger<LoginUser.Handler> logger,
                IMapper mapper)
            {
                _userManager = userManager;
                _appDbContext = appDbContext;
                _tokenService = tokenService;
                _logger = logger;
                _mapper = mapper;
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
                return Result.Success(user);
            }
        }



    }
}
