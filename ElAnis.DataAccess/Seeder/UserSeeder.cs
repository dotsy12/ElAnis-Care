using ElAnis.Entities.Models.Auth.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<User> _userManager)
        {
            var usersCount = await _userManager.Users.CountAsync();
            if (usersCount <= 0)
            {
                var adminUser = new User()
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    PhoneNumber = "01224309198",
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(adminUser, "P@ssw0rd123Pass");
                await _userManager.AddToRoleAsync(adminUser, "Admin");

				var ProviderUser = new User()
				{
					UserName = "Provider",
					Email = "Provider@gmail.com",
					PhoneNumber = "010274804892",
					EmailConfirmed = true,
				};
				await _userManager.CreateAsync(ProviderUser, "P@ssw0rd123Pass");
				await _userManager.AddToRoleAsync(ProviderUser, "Provider");

				var User = new User()
				{
					UserName = "Dotse",
					Email = "dotse@gmail.com",
					PhoneNumber = "01158905589",
					EmailConfirmed = true,
				};
				await _userManager.CreateAsync(User, "P@ssw0rd123Pass");
				await _userManager.AddToRoleAsync(User, "User");
			}

        }
    }
}
