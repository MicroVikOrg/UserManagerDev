
namespace UserManagerDev.Users
{
    public interface IPasswordHasher
    {
        public string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
