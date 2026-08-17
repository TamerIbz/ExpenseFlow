using System.Diagnostics;
using ExpenseFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFlow.Controllers;

public class HomeController : Controller
{
    private readonly ExpenseDbContext _context;

    public HomeController( ExpenseDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Expenses()
    {
        //.OrderByDescending(e => e.Id)
        var allExpenses = _context.Expenses.Include(e => e.Category).ToList();

        var totalExpenses = allExpenses.Sum(x => x.Amount);
        ViewBag.Expenses = totalExpenses;
        
        return View(allExpenses);
    }
    
    public IActionResult CreateEditExpense(int? id) // pressing on CreateEditExpense btn -> gonna show empty form if id null, 
    {
        ShowCategoryList();

        if (id != null) // show prev data that has already been created (id == id, show id table)
        {
            var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
            return View(expenseInDb);
        }
        return View();
    }

    public IActionResult DeleteExpense(int id)
    {
        var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
        _context.Expenses.Remove(expenseInDb);
        _context.SaveChanges();
        return RedirectToAction("Expenses");
    }

    public IActionResult CreateEditExpenseForm(Expense model) // pressing button to create form with details filled in
    {
         if (string.IsNullOrEmpty(model.Title) || (model.CategoryId == 0 || model.CategoryId == null)) // invalid form
        //if(!model.IsValid) // invalid form
        {
            //invalid
            ShowCategoryList(); // show categories again since page is reloaded
            return View("CreateEditExpense", model);
        };
        
        if (model.Id == 0)
        {
            // creating
            _context.Expenses.Add(model);
        }
        else
        {
            //edit
            _context.Expenses.Update(model);
        }

        _context.SaveChanges();
        return RedirectToAction("Expenses");
    }

    private void ShowCategoryList()
    {
        ViewBag.Categories = new SelectList(
            _context.Categories,
            "Id",
            "Name");
    }

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