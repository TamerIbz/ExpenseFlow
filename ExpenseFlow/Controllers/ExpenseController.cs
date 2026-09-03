using ExpenseFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

[Authorize]
public class ExpenseController : Controller
{
    private readonly ExpenseDbContext _context;
    private readonly UserManager<Users> _userManager;
    
    private const string CreateEditExpenseName = "CreateEditExpense";
    private const string ExpensesName = "Index";

    public ExpenseController( ExpenseDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId   = _userManager.GetUserId(User);
        
        var allExpenses = await _context.Expenses
            .Where(e=>e.UserId == userId)
            .Include(e => e.Category)
            .OrderBy(i => i.Id)
            .ToListAsync(); 
        
        var totalExpenses = allExpenses.Sum(x => x.Amount);
        ViewBag.TotalExpenses = totalExpenses;
        
        return View(allExpenses);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CreateEditExpense(int? id) // display form
    {
        await ShowCategoryList();

        if (id != null) // show prev data that has already been created (id == id, show id table)
        {
            var userId   = _userManager.GetUserId(User);
            var expenseInDb = await _context.Expenses.SingleOrDefaultAsync(expense => expense.Id == id && expense.UserId == userId);

            if (expenseInDb == null) return NotFound();
            return View(expenseInDb);
        }
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var userId   = _userManager.GetUserId(User);
        
        var expenseInDb = await _context.Expenses.SingleOrDefaultAsync(expense => expense.Id == id && expense.UserId == userId);
        if (expenseInDb == null)
        {
            return NotFound();
        }
        
        _context.Expenses.Remove(expenseInDb);
        await _context.SaveChangesAsync();
        return RedirectToAction(ExpensesName);
    } // delete expense 

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateEditExpenseForm(Expense model) // save expense form
    {
        //(!ModelState.IsValid) || 
        if (string.IsNullOrWhiteSpace(model.Title) || (model.CategoryId == 0 || model.CategoryId == null)) // invalid form
        {
            //invalid
            await ShowCategoryList(); // show categories again since page is reloaded
            return View(CreateEditExpenseName, model);
        };
        
        var userId   = _userManager.GetUserId(User);
        if (model.Id == 0)
        {
            // creating
            model.UserId = userId;
            _context.Expenses.Add(model);
            
        }
        else
        {
            //edit
            // if(model.UserId == userId)
            // _context.Expenses.Update(model);

            var expenseInDb = await _context.Expenses.SingleOrDefaultAsync(e => e.Id == model.Id && e.UserId == userId);
            if (expenseInDb == null)
                return NotFound();
            
            expenseInDb.Title = model.Title;
            expenseInDb.Amount = model.Amount;
            expenseInDb.CategoryId = model.CategoryId;
            expenseInDb.Date = model.Date;
            expenseInDb.PaymentMethod = model.PaymentMethod;
            expenseInDb.RecurringType = model.RecurringType;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(ExpensesName);
    }

    private async Task ShowCategoryList()
    {
        ViewBag.Categories = new SelectList(await
                _context.Categories.ToListAsync(),
            nameof(Category.Id),
            nameof(Category.Name));
    } // show cat list
}