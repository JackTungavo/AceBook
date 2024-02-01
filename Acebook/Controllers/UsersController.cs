using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;

namespace acebook.Controllers;

public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [Route("/signup")]
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [Route("/users")]
    [HttpPost]
    public RedirectResult Create(User user) {
      AcebookDbContext dbContext = new AcebookDbContext();
      user.ProfileImage = "https://upload.wikimedia.org/wikipedia/commons/6/65/No-Image-Placeholder.svg";
      dbContext.Users.Add(user);
      dbContext.SaveChanges();
      return new RedirectResult("/signin");
    }

    [Route("/profile/{id}")]
    [HttpGet]

    public IActionResult Profile(int id)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
        User user = dbContext.Users.Find(id);
        return View(user);
    }

    [Route("/updateprofile")]
    [HttpPost]

    public RedirectResult UpdateProfile(string newName, string newPhoto, string newBio)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int currentUserId = HttpContext.Session.GetInt32("user_id").Value;

        User user = dbContext.Users.Find(currentUserId);
        user.Name = newName;
        user.Bio = newBio;
        user.ProfileImage = newPhoto;
        dbContext.Users.Update(user);
        dbContext.SaveChanges();

        return new RedirectResult($"/profile/{currentUserId}");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
