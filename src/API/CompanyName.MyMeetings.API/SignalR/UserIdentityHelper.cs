using System.Security.Claims;

namespace CompanyName.MyMeetings.API.SignalR
{
    /// <summary>
    /// Helper class for extracting user information from claims.
    /// </summary>
    public static class UserIdentityHelper
    {
        /// <summary>
        /// Extracts the user ID from ClaimsPrincipal.
        /// Prioritizes NameIdentifier claim for consistency across authentication schemes.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal to extract user ID from.</param>
        /// <returns>The user ID or null if not found.</returns>
        public static string GetUserId(ClaimsPrincipal user)
        {
            if (user == null)
            {
                return null;
            }

            // First try to get NameIdentifier claim (most common for user ID)
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fallback to Name claim if NameIdentifier is not available
            if (string.IsNullOrEmpty(userId))
            {
                userId = user.Identity?.Name;
            }

            return userId;
        }
    }
}
