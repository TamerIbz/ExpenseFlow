using System.ComponentModel.DataAnnotations;

namespace mvcPactice01.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    [Required] public string? Description { get; set; }
    
    
}