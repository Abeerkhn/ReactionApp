using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Linq;
using TestApp.DbContext;
using TestApp.Model;
using TestApp.Services;

namespace TestApp.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MainContext>();

                // Apply pending migrations
                context.Database.Migrate();

                // Seed data
              //  SeedRoles(context);
                SeedUsers(context);

                context.SaveChanges();
            }
        }

       

        private static void SeedUsers(MainContext context)
        {
            if (!context.Users.Any(u => u.EmailOrPhoneNumber == "admin@example.com"))
            {
                var hashedPassword = PasswordService.HashPassword("Admin@123");
                context.Users.Add(new Users
                {
                    FirstName = "Admin User",
                    EmailOrPhoneNumber = "admin@example.com",
                    Password =  hashedPassword, // Ideally, use a password hasher
                   
                });
            }
        }
    }
}
