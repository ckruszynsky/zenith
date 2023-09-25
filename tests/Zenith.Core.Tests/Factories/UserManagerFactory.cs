using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Tests.Factories
{
    public static class UserManagerFactory
    {
        public static UserManager<ZenithUser> Create(ZenithUser user)
        {
            var userStoreMock = new Mock<IUserStore<ZenithUser>>();
            var userEmailStoreMock = new Mock<IUserEmailStore<ZenithUser>>();

            // Setup store mock calls
            userStoreMock.Setup(s => s.CreateAsync(user, CancellationToken.None))
                .Returns(Task.FromResult(IdentityResult.Success));
            userEmailStoreMock.Setup(s => s.FindByEmailAsync(user.Email, CancellationToken.None))
                .Returns((Task<ZenithUser>)null);

            var options = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions
            {
                Lockout =
                {
                    AllowedForNewUsers = false
                }
            };
            options.Setup(o => o.Value).Returns(identityOptions);
            var userValidators = new List<IUserValidator<ZenithUser>>();
            var validator = new Mock<IUserValidator<ZenithUser>>();
            userValidators.Add(validator.Object);
            var passwordValidators = new List<PasswordValidator<ZenithUser>>
            {
                new PasswordValidator<ZenithUser>()
            };
            var userManager = new UserManager<ZenithUser>(
                userEmailStoreMock.Object,
                options.Object,
                new PasswordHasher<ZenithUser>(),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                NullLogger<UserManager<ZenithUser>>.Instance);
            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<ZenithUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }
    }
}
