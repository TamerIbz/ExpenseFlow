using ExpenseFlow.Models;
using ExpenseFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ExpenseDbContext _context;
    private readonly UserManager<Users> _userManager;

    public AdminController(ExpenseDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IActionResult AdminPage()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserAccount(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var adminUser = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Cannot find account with ID {userId}");
        if (adminUser != null && user.Id == adminUser.Id)
        {
            return BadRequest("You cannot delete your own admin account.");
        }
        
        var expenses = await _context.Expenses.Where(e => e.UserId == user.Id).ToListAsync();
        _context.Expenses.RemoveRange(expenses);
        await _context.SaveChangesAsync();
        
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest();
        }
        
        TempData["DeletionAccountSuccessMessage"] = "Account was deleted successfully.";
        return RedirectToAction(nameof(AdminPage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExpense(int expenseId)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == expenseId);
        if(expense == null)
        {
            return BadRequest($"Cannot find Expense with Id {expenseId}");
        }
        _context.Expenses.Remove(expense);
        await  _context.SaveChangesAsync();
       
        TempData["DeletionExpenseSuccessMessage"] = "Expense was deleted successfully.";
       
        return RedirectToAction(nameof(AdminPage));
    }
}