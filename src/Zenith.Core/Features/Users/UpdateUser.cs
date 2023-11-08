using System.Net;
using System.Security.Claims;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zenith.Common.Exceptions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Users.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Users
{
    public class UpdateUser
    {
        public record Command(UpdateUserDto UpdateUserDto) : IRequest<Result<UserViewModel>>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(u => u.UpdateUserDto.Email).EmailAddress();
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

                var claimsPrincipal = _currentUserContext.GetCurrentUserContext();
                var currentUser = _appDbContext.Users.Single(u => u.UserName == claimsPrincipal.UserName);
                var updateUserDto = request.UpdateUserDto;

                if (IsRequestPropertyAvailableForUpdate(updateUserDto.Email, currentUser.Email))
                {
                    var priorExistingEmail = await _userManager.FindByEmailAsync(updateUserDto.Email);
                    if (priorExistingEmail != null)
                    {
                        throw new ApiException($"Email {updateUserDto.Email} is already in use", HttpStatusCode.BadRequest);                            
                    }

                    issueNewToken = true; //flip flag for the new email
                }

                if (IsRequestPropertyAvailableForUpdate(updateUserDto.Username, currentUser.UserName))
                {
                    var priorExistingUsername = await _userManager.FindByNameAsync(updateUserDto.Username);
                    if (priorExistingUsername != null)
                    {
                       throw new ApiException($"Username {updateUserDto.Username} is already in use", HttpStatusCode.BadRequest);
                    }
                    // Flip the issue token flag for the new username
                    issueNewToken = true;
                }

                if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                {
                    await _userManager.RemovePasswordAsync(currentUser);
                    var userStore = new UserStore<ZenithUser>(_appDbContext);
                    await userStore.SetPasswordHashAsync(
                        currentUser,
                        new PasswordHasher<ZenithUser>().HashPassword(currentUser, updateUserDto.Password), CancellationToken.None);
                }

                currentUser.Email = string.IsNullOrEmpty(updateUserDto.Email) ? currentUser.Email : updateUserDto.Email;
                currentUser.UserName = string.IsNullOrEmpty(updateUserDto.Username) ? currentUser.UserName : updateUserDto.Username;
                currentUser.Bio = updateUserDto.Bio ?? currentUser.Bio;
                currentUser.Image = updateUserDto.Image ?? currentUser.Image;

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
