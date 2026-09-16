using System.Diagnostics;
using System.Globalization;
using ExpenseFlow.Models;
using ExpenseFlow.Services.Home;
using ExpenseFlow.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

public class HomeController : Controller
{
     private readonly IHomeService  _homeService;
     private readonly UserManager<Users> _userManager;
    
    public HomeController( IHomeService homeService, UserManager<Users> userManager)
    {
        _homeService = homeService;
        _userManager = userManager;
    }
 
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(new HomeViewModel());
        }
        
        var userId   = _userManager.GetUserId(User);
        if (userId == null)
            return View(new HomeViewModel());
        
        var model = await _homeService.SyncDashboardAsync(userId);
        var user = await _userManager.GetUserAsync(User);

       var username = user?.FullName;
       if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(username))
       {
           ViewBag.UserName  = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(username.ToLower());
       }
        
        return View(model);
    }
    
    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}