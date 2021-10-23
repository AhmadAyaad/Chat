using System.Linq;
using System.Threading.Tasks;
using ChatTrials;
using Microsoft.AspNetCore.Identity;

public class ApplicationDbContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager,
                                                     RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.NormalUser.ToString()));

            //Seed Default User
            var defaultUser = new ApplicationUser {
                            UserName = Authorization.default_username,
                            Email = Authorization.default_email,
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.default_password);
                await userManager.AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
            }
        }
    }