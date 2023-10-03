using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Users
{
    public class UpdateUser
    {
        public record Command(string? Email = "", string? Username = "", string? Image = "", string? Bio = "", string? Password = "") : IRequest<Result<UserViewModel>>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(u => u.Email).EmailAddress();
            }
        }

        public class Handler : IRequestHandler<Command, Result<UserViewModel>>
        {
            private readonly ICurrentUserContext _currentUserContext;
            private readonly IMapper _mapper;
            private readonly UserManager<ZenithUser> _userManager;
            private readonly AppDbContext _appDbContext;
            private readonly ITokenService _tokenService;
            private readonly IMediator _mediator;

            public Handler(
                ICurrentUserContext currentUserContext,
                IMapper mapper,
                UserManager<ZenithUser> userManager,
                AppDbContext appDbContext,
                ITokenService tokenService,
                IMediator mediator)
            {
                _currentUserContext = currentUserContext;
                _mapper = mapper;
                _userManager = userManager;
                _appDbContext = appDbContext;
                _tokenService = tokenService;
                _mediator = mediator;
            }

            public async Task<Result<UserViewModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                var issueNewToken = false; //initialize flag to retrieve a new token on password reset

                var currentUser = await _currentUserContext.GetCurrentUserContext();

                if (IsRequestPropertyAvailableForUpdate(request.Email, currentUser.Email))
                {
                    var priorExistingEmail = await _userManager.FindByEmailAsync(request.Email);
                    if (priorExistingEmail != null)
                    {
                        return Result.Error($"Email {request.Email} is already in use");
                    }

                    issueNewToken = true; //flip flag for the new email
                }

                if (IsRequestPropertyAvailableForUpdate(request.Username, currentUser.UserName))
                {
                    var priorExistingUsername = await _userManager.FindByNameAsync(request.Username);
                    if (priorExistingUsername != null)
                    {
                        return Result.Error($"Username {request.Username} is already in use");
                    }
                    // Flip the issue token flag for the new username
                    issueNewToken = true;
                }

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    await _userManager.RemovePasswordAsync(currentUser);
                    var userStore = new UserStore<ZenithUser>(_appDbContext);
                    await userStore.SetPasswordHashAsync(
                        currentUser,
                        new PasswordHasher<ZenithUser>().HashPassword(currentUser, request.Password), CancellationToken.None);
                }

                currentUser.Email = string.IsNullOrEmpty(request.Email) ? currentUser.Email : request.Email;
                currentUser.UserName = string.IsNullOrEmpty(request.Username) ? currentUser.UserName : request.Username;
                currentUser.Bio = request.Bio ?? currentUser.Bio;
                currentUser.Image = request.Image ?? currentUser.Image;

                await _userManager.UpdateAsync(currentUser);

                var userViewModel = _mapper.Map<UserViewModel>(currentUser);

                userViewModel.Token = issueNewToken ?
                    _tokenService.CreateToken(currentUser)
                    : _currentUserContext.GetCurrentUserToken();

                await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                {
                    ActivityType = ActivityType.UserUpdated,
                    TransactionType = TransactionType.ZenithUser,
                    TransactionId = currentUser.Id
                }), cancellationToken);

                return Result.Success(userViewModel);
            }

            private static bool IsRequestPropertyAvailableForUpdate(string requestProperty, string currentProperty)
            {
                return !string.IsNullOrWhiteSpace(requestProperty) &&
                       !string.Equals(requestProperty, currentProperty, StringComparison.OrdinalIgnoreCase);
            }
        }



    }
}
