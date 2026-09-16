using ExpenseFlow.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpenseFlow.Services;

public class SeedService
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        var logger= scope.ServiceProvider.GetRequiredService<ILogger<SeedService>> ();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        try
        {
            logger.LogInformation("Ensuring the database is created");
            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Seeding roles");
            await AddRoleAsync(roleManager, "Admin");
            await AddRoleAsync(roleManager, "User");

            logger.LogInformation("Seeding admin user.");
            var adminEmail = config["Admin:Email"];
            var adminPassword = config["Admin:Password"];


            var adminUser = await userManager.FindByEmailAsync(adminEmail!);
            if (adminUser != null)
            {
               // if need to update admin, 
               // await userManager.UpdateAsync(adminUser);
            }
            else
            {
                var newAdminUser = new Users
                {
                    FullName = "ExpenseFlow Admin",
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail!.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(newAdminUser, adminPassword!);
                if (result.Succeeded)
                {
                    logger.LogInformation("Assigning Admin role to admin user");
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
                else
                {
                    logger.LogError("Failed to create admin user {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error has occured");
        }
    }

    private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create role '{roleName}' : {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}