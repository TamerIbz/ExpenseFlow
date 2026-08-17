using Microsoft.EntityFrameworkCore;
namespace ExpenseFlow.Models;

public class ExpenseDbContext : DbContext
{
    public DbSet<Expense> Expenses { get; set; }

    public DbSet<Category> Categories { get; set; }

    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Food" },
            new Category { Id = 2, Name = "Entertainment" },
            new Category { Id = 3, Name = "Shopping" },
            new Category { Id = 4, Name = "Transport" },
            new Category { Id = 5, Name = "Bills" }
        );
    }
}