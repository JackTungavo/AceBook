using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    AcebookDbContext dbContext = new AcebookDbContext();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Route("/")]
    public IActionResult Index()
    {
        //HttpContext.Session.GetInt32("user_id").Value = 0;
        int? userId = HttpContext.Session.GetInt32("user_id");
        if (userId.HasValue)
        {
            HttpContext.Session.SetInt32("user_id", 0);
        }

        return View();
    }

    [Route("/privacy")]
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
