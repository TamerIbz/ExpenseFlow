using ExpenseFlow.Models;
using ExpenseFlow.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ExpenseFlow.Services.Account;

public interface IAccountService
{
    Task<(IdentityResult Result, Users User)>  RegisterAsync(RegisterViewModel model);
    Task DeleteAccountAsync(Users user);
}