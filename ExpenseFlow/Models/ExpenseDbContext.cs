using Microsoft.EntityFrameworkCore;
namespace mvcPactice01.Models;

public class ExpenseDbContext : DbContext
{
    public DbSet<Expense> Expenses { get; set; }

    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
    {
        
    }
}