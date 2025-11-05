using Users.Domain.Entities;

namespace Users.Core.Repositories
{
    public interface IKeycloakRepository
    {
        Task<string> GetTokenAsync();
        Task<string> CreateUserAsync(object user, string token);
        //Task AssignTypeToUserAsync(string keycloakUserId, UserType userType, string token);
        Task<string?> GetUserIdAsync(string keycloakUserId, string token);
        Task<string> UpdateUserAsync(object user, string keycloakUserId, string token);
        Task SendVerificationEmailAsync(string keycloakUserId, string token);
        Task SendPasswordResetEmailAsync(string keycloakUserId, string token);
    }
}