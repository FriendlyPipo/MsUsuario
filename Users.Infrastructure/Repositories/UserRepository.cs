using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Core.Repositories;
using Users.Infrastructure.Database;

namespace Users.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly UserDbContext _dbContext;

        public UserRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Operaciones de escritura

        public async Task CreateAsync(User user)
        {
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        //Operaciones de lectura

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.User.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(UserId userId)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByEmailAsync(UserEmail email)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.UserEmail == email);
        }

        public async Task<List<User>> GetByTypeAsync(UserType userType)
        {
            return await _dbContext.User.Where(u => u.UserType == userType).ToListAsync();
        }
    }
}