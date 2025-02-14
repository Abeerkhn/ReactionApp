
    using System.Threading.Tasks;

    namespace TestApp.Repositories
    {
        public interface IUserRepository
        {
            Task<string> RegisterUser(string fullName,string email, string password);
            Task<string?> AuthenticateUser(string email, string password);
            Task<string> UpdateProfile(long userId, string? newFirstName, string? newLastName, string? newEmailOrPhone);
        }
    }


