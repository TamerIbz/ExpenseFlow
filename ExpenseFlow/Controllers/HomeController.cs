using System.Diagnostics;
using System.Globalization;
using ExpenseFlow.Models;
using ExpenseFlow.ViewModels;
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
 
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(new HomeViewModel());
        }
        
        var userId   = _userManager.GetUserId(User);
        var user = await _userManager.GetUserAsync(User);
        
        var allExpenses = await _context.Expenses
            .Where(e=>e.UserId == userId)
            .Include(e => e.Category)
            .OrderBy(i => i.Id)
            .ToListAsync();
        
        var allCategories = allExpenses
            .GroupBy(e => e.Category!.Name)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount)
            }).ToList();
        
        var categoryNames = allCategories
            .Select(x => x.Category)
            .ToList();
        
        var categoryTotalAmount = allCategories
            .Select(x => x.Total)
            .ToList();
        
        var totalSpent = allExpenses.Sum(e => e.Amount);
        var topCategory= allCategories
            .OrderByDescending(i => i.Total)
            .FirstOrDefault();
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        var monthTotal =allExpenses.Where(e =>
            e.Date.HasValue 
            && e.Date.Value.Year == today.Year 
            && e.Date.Value.Month == today.Month).Sum(e=>e.Amount);


       var model = new HomeViewModel()
       {
           Expenses = allExpenses,
           TotalSpent = totalSpent,
           TopCategory = topCategory?.Category,
           MonthTotal = monthTotal,
           CategoryTotals = categoryTotalAmount,
           CategoryNames = categoryNames

       };

       var username = user?.FullName;
       if (username != null && !string.IsNullOrEmpty(username))
       {
           username = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(username.ToLower());
           ViewBag.UserName = username;
       }
        
        return View(model);
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