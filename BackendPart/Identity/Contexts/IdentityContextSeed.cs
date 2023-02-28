using Identity.Models;
using Identity.Constants;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace Identity.Contexts
{
    public class IdentityContextSeed
    {
        public static async Task SeedEssentialAsync
            (UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync
            (
                new IdentityRole(Authorization.Roles.Administrator.ToString())
            );
            await roleManager.CreateAsync
            (
                new IdentityRole(Authorization.Roles.Manager.ToString())
            );
            await roleManager.CreateAsync
            (
                new IdentityRole(Authorization.Roles.Worker.ToString())
            );

            var defaultUser = new User
            {
                UserName = Authorization.default_username,
                Email = Authorization.default_email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager
                    .CreateAsync(defaultUser, Authorization.default_password);
                await userManager
                    .AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
            }
        }
    }
}
