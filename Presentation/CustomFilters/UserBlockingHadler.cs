using Application.Repositories;
using Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Presentation.CustomFilters
{
    public class UserBlockingHadler : AuthorizationHandler<UserBlockingRequirement>
    {
        private readonly IUserRepository _userRepository;

        public UserBlockingHadler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserBlockingRequirement requirement)
        {
            Claim? claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null)
            {
                context.Fail(new AuthorizationFailureReason(this, "Claim not found"));
                return;
            }

            User? user = await _userRepository.GetByIdAsync(Guid.Parse(claim!.Value));
            if (user is null || user.IsBlocked && !requirement.AllowBlocked)
            {
                context.Fail(new AuthorizationFailureReason(this, "User is blocked or doesn't exist"));
                return;
            }

            context.Succeed(requirement);
        }
    }
}
