using ExpenseFlow.Models;

namespace ExpenseFlow.ViewModels;

public class HomeViewModel
{
    public List<Expense> Expenses { get; set; } = new();
    public decimal TotalSpent { get; set; }
    public decimal MonthTotal { get; set; }
    public string? TopCategory { get; set; }
    public List<String> CategoryNames { get; set; } = new();
    public List<decimal> CategoryTotals { get; set; }=new();
}