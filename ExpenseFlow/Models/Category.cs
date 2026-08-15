namespace mvcPactice01.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public List<Expense> Expenses { get; set; } = [];
}