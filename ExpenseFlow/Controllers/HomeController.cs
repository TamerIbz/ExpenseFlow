using System.Diagnostics;
using ExpenseFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

public class HomeController : Controller
{
     private readonly ExpenseDbContext _context;
    //
    // private const string CreateEditExpenseName = "CreateEditExpense";
    // private const string ExpensesName = "Expenses";
    // private List<Expense>  allExpenses;

    public HomeController( ExpenseDbContext context)
    {
        _context = context;
    }
    
    // what is viewbag, walk through, 
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View();
        }
        var spendingCategory = await _context.Expenses
            .Include(e => e.Category)
            .GroupBy(e => e.Category!.Name)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount)
            }).ToListAsync();
        
        ViewBag.CategoryNames = spendingCategory
            .Select(x => x.Category)
            .ToList();

        ViewBag.CategoryTotals = spendingCategory
            .Select(x => x.Total)
            .ToList();
        
        //  allExpenses = await _context.Expenses.
        //     Include(e => e.Category).
        //     OrderBy(i => i.Id).
        //     ToListAsync();
        //
        // var totalExpenses = allExpenses.Sum(x => x.Amount);
        // ViewBag.TotalExpenses = totalExpenses;

        ViewBag.TotalExpenses = await _context.Expenses.SumAsync(e => e.Amount);
        var topCategory= spendingCategory
            .OrderByDescending(i => i.Total)
            .FirstOrDefault();

        ViewBag.TopCategory = topCategory?.Category;
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        ViewBag.MonthTotal = await _context.Expenses.Where(e =>
            e.Date.HasValue 
            && e.Date.Value.Year == today.Year 
            && e.Date.Value.Month == today.Month).SumAsync(e=>e.Amount);
        
        return View();
    }
    
    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}