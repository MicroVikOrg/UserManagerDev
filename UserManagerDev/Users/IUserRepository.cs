using UserManagerDev.Database.Entities;

namespace UserManagerDev.Users
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string email, ApplicationContext context);
        Task InsertAsync(User user , ApplicationContext context);
        Task<User?> GetByEmailAsync(string email, ApplicationContext context);
    }
}
