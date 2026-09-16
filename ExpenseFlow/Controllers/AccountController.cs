using ExpenseFlow.Models;
using ExpenseFlow.Services;
using ExpenseFlow.Services.Account;
using ExpenseFlow.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

public class AccountController : Controller
{
    public AccountController(SignInManager<Users> signInManager,UserManager<Users> userManager,IEmailService emailService, IAccountService accountService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
        _accountService = accountService;
    }
    
    private readonly SignInManager<Users> _signInManager;
    private readonly UserManager<Users> _userManager;
    private readonly IEmailService _emailService;
    private readonly IAccountService _accountService;

    #region Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Email or password is incorrect");
            return View(model);
        }
        
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError("", "Please confirm your account");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, 
            model.Password, 
            model.RememberMe, 
            false);

        if (!result.Succeeded)
        {
            // default controller=home, page/action=index
            ModelState.AddModelError("", "Email or password is incorrect");
            return View(model);
        }
        
        return RedirectToAction("Index", "Home");
    }
    #endregion Login

    #region Register
    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Users user = new Users
            // {
            //     FullName = model.Name,
            //     Email = model.Email,
            //     UserName = model.Email
            // };
            //
            // var result = await _userManager.CreateAsync(user, model.Password);
            var result = await _accountService.RegisterAsync(model);
            
            if (result.Result.Succeeded)
            {
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(result.User);
                var confirmLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = result.User!.Id,
                    Token = confirmToken
                }, Request.Scheme);
                
                await _emailService.SendConfirmAccountEmailAsync(model.Email, confirmLink!);
                
                return RedirectToAction("EmailSent", "Account");
            }
  
            foreach (var error in result.Result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    #region VerificationEmailSent
    [HttpGet]
    public IActionResult EmailSent() // email sent page
    {
        return View();
    }
    #endregion VerificationEmailSent

    #region ConfirmEmail
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token) // clicked on confirm email link
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null){ return NotFound(); }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded) { return RedirectToAction("Login", "Account"); }
        return View("Error");
    }
    #endregion
    #endregion Signup

    #region ChangePassword 
    [HttpGet]
    public IActionResult VerifyEmailForChangePassword()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> VerifyEmailForChangePassword(VerifyEmailViewModel verifyEmailViewModel)
    {

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(verifyEmailViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(verifyEmailViewModel);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new
            {
                email = verifyEmailViewModel.Email,
                Token = resetToken
            }, Request.Scheme);
            
            await _emailService.SendResetPasswordEmailAsync(verifyEmailViewModel.Email, resetLink!);
            return RedirectToAction("EmailSent", "Account");
        }
        return View(verifyEmailViewModel);
    }
    
    #region ResetPassword
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("VerifyEmailForChangePassword", "Account");
        }

        var model = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
   
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "User not found");
            return View(model);
        }

        var resetResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!resetResult.Succeeded)
        {
            foreach (var error in resetResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        return RedirectToAction("Login");
    }
    #endregion
    #endregion

    #region Account
    [Authorize]
    [HttpGet]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();


        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return BadRequest("You cannot delete your own admin account.");
        }
        
        await _accountService.DeleteAccountAsync(user);
        await _signInManager.SignOutAsync();
        
        return RedirectToAction("Index", "Home");
    }
    #endregion
}
