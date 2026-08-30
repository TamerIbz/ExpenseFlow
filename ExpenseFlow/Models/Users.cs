using Microsoft.AspNetCore.Identity;

namespace ExpenseFlow.Models;

public class Users : IdentityUser
{
    public string? FullName { get; set; }
}