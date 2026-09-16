using ExpenseFlow.Models;
using ExpenseFlow.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Services.Home;

public class HomeService : IHomeService
{
    private readonly ExpenseDbContext _context;

    public HomeService(ExpenseDbContext context)
    {
        _context = context;
    }
    
    public async Task<HomeViewModel> SyncDashboardAsync(string userId)
    {
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
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var model = new HomeViewModel
        {
            Expenses = allExpenses,
            TotalSpent =  allExpenses.Sum(e => e.Amount),
            
            TopCategory = allCategories
            .OrderByDescending(i => i.Total)
            .FirstOrDefault()?.Category,
            
            MonthTotal =allExpenses.Where(e =>
            e.Date.HasValue 
            && e.Date.Value.Year == today.Year 
            && e.Date.Value.Month == today.Month).Sum(e=>e.Amount),
            
            CategoryTotals = allCategories
            .Select(x => x.Total)
            .ToList(),
            
            CategoryNames = allCategories
            .Select(x => x.Category)
            .ToList()
        };

        return model;
    }
}