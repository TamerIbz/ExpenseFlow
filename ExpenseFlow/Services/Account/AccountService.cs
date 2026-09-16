using ExpenseFlow.Models;
using ExpenseFlow.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Services.Account;

public class AccountService : IAccountService
{
    public  AccountService(ExpenseDbContext context,UserManager<Users>  userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private readonly UserManager<Users> _userManager;
    private readonly ExpenseDbContext _context;
    public async Task<(IdentityResult Result, Users User)> RegisterAsync(RegisterViewModel model)
    {
        Users user = new Users
        {
            FullName = model.Name,
            Email = model.Email,
            UserName = model.Email
        };
        
       var result = await _userManager.CreateAsync(user, model.Password);
       return (result, user);
    }

    public async Task DeleteAccountAsync(Users user)
    {
        var expenses = await _context.Expenses.Where(
            e => e.UserId == user.Id).ToListAsync();
        
        _context.Expenses.RemoveRange(expenses);
        await _context.SaveChangesAsync();
        
        await _userManager.DeleteAsync(user);
    }
}