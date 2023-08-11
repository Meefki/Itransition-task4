using Microsoft.AspNetCore.Authorization;

namespace Presentation.CustomFilters
{
    public class UserBlockingRequirement : IAuthorizationRequirement
    {
        public UserBlockingRequirement()
        {
            AllowBlocked = false;
        }

        public bool AllowBlocked { get; init; }
    }
}
