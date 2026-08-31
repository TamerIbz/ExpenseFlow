using ExpenseFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

public class ExpenseController : Controller
{
    private readonly ExpenseDbContext _context;
    private readonly UserManager<Users> _userManager;
    
    private const string CreateEditExpenseName = "CreateEditExpense";
    private const string ExpensesName = "Index";
    // private List<Expense> AllExpenses;
    
    public ExpenseController( ExpenseDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [Authorize]
    public async Task<IActionResult> Index()
    {
        
        // Console.WriteLine("called");
        //.OrderByDescending(e => e.Id)
    
        var allExpenses = await _context.Expenses.
            Include(e => e.Category).
            OrderBy(i => i.Id).
            ToListAsync();
        ;
        
        var totalExpenses = allExpenses.Sum(x => x.Amount);
        ViewBag.TotalExpenses = totalExpenses;
        
        return View(allExpenses);
    }
    
    public async Task<IActionResult> CreateEditExpense(int? id) // pressing on CreateEditExpense btn -> gonna show empty form if id null, ->display form
    {
        await ShowCategoryList();

        if (id != null) // show prev data that has already been created (id == id, show id table)
        {
            var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
            return View(expenseInDb);
        }
        return View();
    }

    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
        if (expenseInDb != null) _context.Expenses.Remove(expenseInDb);
        await _context.SaveChangesAsync();
        return RedirectToAction(ExpensesName);
    } // delete expense 

    public async Task<IActionResult> CreateEditExpenseForm(Expense model) // pressing button to create form with details filled in -? save form
    {
        //if(!model.IsValid) // invalid form
         if (string.IsNullOrWhiteSpace(model.Title) || (model.CategoryId == 0 || model.CategoryId == null)) // invalid form
        {
            //invalid
            await ShowCategoryList(); // show categories again since page is reloaded
            return View(CreateEditExpenseName, model);
        };
        
        if (model.Id == 0)
        {
            // creating
            _context.Expenses.Add(model);
        }
        else
        {
            //edit
            _context.Expenses.Update(model);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(ExpensesName);
    }

    private async Task ShowCategoryList()
    {
        try
        {
            ViewBag.Categories = new SelectList(await
                    _context.Categories.ToListAsync(),
                nameof(Category.Id),
                nameof(Category.Name));
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    } // show cat list
}