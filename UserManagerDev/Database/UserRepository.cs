using Microsoft.EntityFrameworkCore;
using UserManagerDev.Database.Entities;
using UserManagerDev.Users;

namespace UserManagerDev.Database
{
    public sealed class UserRepository : IUserRepository
    {
        public async Task<bool> ExistsAsync(string email, ApplicationContext context)
        {
                return await context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email, ApplicationContext context)
        {
                return await context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task InsertAsync(User user, ApplicationContext context)
        {
                context.Users.Add(user);
                await context.SaveChangesAsync();
        }
    }
}
