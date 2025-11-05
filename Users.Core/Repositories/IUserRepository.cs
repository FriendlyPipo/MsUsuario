using Users.Domain.Entities;

namespace Users.Core.Repositories
{
    public interface IUserRepository
    {
        Task UpdateAsync(User user); 
        Task CreateAsync(User user); 
        Task<User?> GetByIdAsync(UserId userId);

        Task<List<User>> GetAllAsync();

        Task<User?> GetByEmailAsync(UserEmail email);

        Task<List<User>> GetByTypeAsync(UserType userType);
    }
}