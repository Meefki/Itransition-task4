using Application.Repositories;
using Domain;
using System.Security.Claims;

namespace Presentation.Helpers
{
    public static class IdentityHelper
    {
        public static async Task<User?> GetAuthenticatedUserAsync(HttpContext context, IUserRepository userRepository)
        {
            ClaimsPrincipal claimUser = context.User;
            Claim? claim = claimUser.FindFirst(ClaimTypes.NameIdentifier);
            User? user = null;
            if ((claimUser.Identity?.IsAuthenticated ?? false) && claim is not null)
                user = await userRepository.GetByIdAsync(Guid.Parse(claim!.Value));

            return user;
        } 
    }
}
