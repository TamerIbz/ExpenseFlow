using System.ComponentModel.DataAnnotations;
using ExpenseFlow.Models.Enums;

namespace ExpenseFlow.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "*REQUIRED*")][StringLength(50, MinimumLength = 2)] public string? Title { get; set; } = string.Empty;
    // [StringLength(150, MinimumLength = 5)]public string? Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "*REQUIRED*")]public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public DateOnly? Date { get; set; }
    public PaymentMethods PaymentMethod { get; set; } = PaymentMethods.DebitCard;
    public RecurringType RecurringType { get; set; } = RecurringType.None;

}