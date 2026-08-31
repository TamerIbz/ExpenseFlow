using ExpenseFlow.Models;
using ExpenseFlow.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseFlow.Controllers;

public class AccountController : Controller
{
    public AccountController(SignInManager<Users> signInManager,UserManager<Users> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    private readonly SignInManager<Users> _signInManager;
    private readonly UserManager<Users> _userManager;
    
    // GET
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                // default controller=home, page/action=index
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(model);
            }
        }
        return View(model);
    }
    
    [HttpGet]
    public IActionResult SignUp()
    {
      //  Console.WriteLine("Called111");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            Users users = new Users
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email
            };
        
            var result = await _userManager.CreateAsync(users, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
        
                return View(model);
            }
        
        
        }
        return View(model);
    }
    
    [HttpGet]
    public IActionResult VerifyEmail()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email is invalid");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }
        return View(model);
    }
    
    [HttpGet]
    public IActionResult ChangePassword(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("VerifyEmail", "Account");
        }

        return View(new ChangePasswordViewModel { Email = username });
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Email not found!");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong, try again!");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
