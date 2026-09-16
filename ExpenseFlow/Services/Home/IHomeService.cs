using ExpenseFlow.ViewModels;

namespace ExpenseFlow.Services.Home;

public interface IHomeService
{
    Task<HomeViewModel> SyncDashboardAsync(string userId);
}