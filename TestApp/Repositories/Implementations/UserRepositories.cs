

    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
using TestApp.DbContext;
using TestApp.Model;
using TestApp.Services;

namespace TestApp.Repositories
    {
        public class UserRepository : IUserRepository
        {
            private readonly MainContext _context;
            private readonly JwtService _jwtService;

            public UserRepository(MainContext context, JwtService jwtService)
            {
                _context = context;
                _jwtService = jwtService;
            }

            public async Task<string> RegisterUser(string fullName,string email, string password)
            {
                if (await _context.Users.AnyAsync(u => u.EmailOrPhoneNumber == email))
                    return "Email already exists.";

                var hashedPassword = PasswordService.HashPassword(password);
                var user = new Users { EmailOrPhoneNumber = email, Password = hashedPassword, FirstName=fullName };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return "User registered successfully!";
            }

            public async Task<string?> AuthenticateUser(string email, string password)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.EmailOrPhoneNumber == email);
                if (user == null || !PasswordService.VerifyPassword(password, user.Password))
                    return null;

                return _jwtService.GenerateToken(user);
            }
        public async Task<string> UpdateProfile(long userId, string? newFirstName, string? newLastName, string? newEmailOrPhone)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
        return "User not found.";

    // Check if the new email or phone is already taken (only if a new value is provided)
    if (!string.IsNullOrEmpty(newEmailOrPhone) &&
        await _context.Users.AnyAsync(u => u.EmailOrPhoneNumber == newEmailOrPhone && u.Id != userId))
    {
        return "Email or phone number already in use.";
    }

    // Update only non-null values
    if (!string.IsNullOrEmpty(newFirstName))
        user.FirstName = newFirstName;

    if (!string.IsNullOrEmpty(newLastName))
        user.LastName = newLastName;

    if (!string.IsNullOrEmpty(newEmailOrPhone))
        user.EmailOrPhoneNumber = newEmailOrPhone;

    user.ModifiedDate = DateTime.UtcNow;

    _context.Users.Update(user);
    await _context.SaveChangesAsync();

    return "Profile updated successfully!";
}

        }



    }


