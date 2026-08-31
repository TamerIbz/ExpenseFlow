using System.Diagnostics;
using ExpenseFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

public class HomeController : Controller
{
     private readonly ExpenseDbContext _context;
     private readonly UserManager<Users> _userManager;
    

    public HomeController( ExpenseDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // what is viewbag, walk through, 
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View();
        }
        
        var userId   = _userManager.GetUserId(User);
        var allExpenses = await _context.Expenses
            .Where(e=>e.UserId == userId)
            .Include(e => e.Category)
            .OrderBy(i => i.Id)
            .ToListAsync();


        // categories
        var spendingCategory = allExpenses
            .GroupBy(e => e.Category!.Name)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount)
            }).ToList();
        
        // all cat names
        ViewBag.CategoryNames = spendingCategory
            .Select(x => x.Category)
            .ToList();

        // total spending cat name
        ViewBag.CategoryTotals = spendingCategory
            .Select(x => x.Total)
            .ToList();

        
        // total expense digit
        ViewBag.TotalExpenses = allExpenses.Sum(e => e.Amount);
        var topCategory= spendingCategory
            .OrderByDescending(i => i.Total)
            .FirstOrDefault();

        ViewBag.TopCategory = topCategory?.Category;
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        ViewBag.MonthTotal =allExpenses.Where(e =>
            e.Date.HasValue 
            && e.Date.Value.Year == today.Year 
            && e.Date.Value.Month == today.Month).Sum(e=>e.Amount);
        
        return View(allExpenses);
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