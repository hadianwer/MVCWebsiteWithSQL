using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using static System.Console;
using System.Reflection.Metadata.Ecma335;

namespace MvcPractical.Controllers;

public class RolesController : Controller
{
    private string AdminRole = "Administrators";
    private string UserEmail = "has@hadi.com";
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<IdentityUser> userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!(await roleManager.RoleExistsAsync(AdminRole)))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }
        IdentityUser user = await userManager.FindByEmailAsync(UserEmail);
        if (user == null)
        {
            user = new();
            user.Email = UserEmail;
            user.UserName = UserEmail;

            IdentityResult result = await userManager.CreateAsync(user, "Pa$$w0rd");
            if (result.Succeeded)
            {
                WriteLine($"{user.UserName}user created succefully");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    WriteLine(error.Description);
                }
            }
        }
        if (!await userManager.IsInRoleAsync(user, AdminRole))
        {
            IdentityResult result = await userManager.AddToRoleAsync(user, AdminRole);
            if (result.Succeeded)
            {
                WriteLine($"User {user.UserName} added successfully to {AdminRole}.");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    WriteLine(error.Description);
                }
            }
        }
        return Redirect("/");
    }
    
        
}
