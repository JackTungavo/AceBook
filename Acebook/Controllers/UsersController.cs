using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;

namespace acebook.Controllers;

public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    AcebookDbContext dbContext = new AcebookDbContext();

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    public void UnfollowUser(int UserIdIWantToUnfollow)
    {
        int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
        User user = dbContext.Users.Find(currentUserId);
        User userToUnfollow = dbContext.Users.Find(UserIdIWantToUnfollow);

        user.UsersIFollow = user.UsersIFollow.Replace("'"+UserIdIWantToUnfollow.ToString()+"',","");
        userToUnfollow.UsersFollowingMe = userToUnfollow.UsersFollowingMe.Replace("'"+currentUserId.ToString()+"',","");

        dbContext.Users.Update(user);
        dbContext.Users.Update(userToUnfollow);
        dbContext.SaveChanges();
    }

    public void FollowUser(int UserIdIWantToFollow)
    {
        int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
        User user = dbContext.Users.Find(currentUserId);
        User userToFollow = dbContext.Users.Find(UserIdIWantToFollow);
        Console.WriteLine($"user.UsersIFollow: {user.UsersIFollow.ToString()}");
        if (user.UsersIFollow.Contains("'"+UserIdIWantToFollow.ToString()+"',") == false) 
        {
            user.UsersIFollow += "'"+UserIdIWantToFollow.ToString()+"',";
            dbContext.Users.Update(user);
            userToFollow.UsersFollowingMe += "'"+currentUserId.ToString()+"',";
            dbContext.Users.Update(userToFollow);
        }
        else
        {
            user.UsersIFollow = user.UsersIFollow.Replace("'"+UserIdIWantToFollow.ToString()+"',","");
            dbContext.Users.Update(user);
            userToFollow.UsersFollowingMe = userToFollow.UsersFollowingMe.Replace("'"+currentUserId.ToString()+"',","");
            dbContext.Users.Update(userToFollow);
        }
        dbContext.SaveChanges();
    }

    public int GetNumberOfUserFollowers(int UserId)
    {
        User CurrentUser = dbContext.Users.Find(UserId);
        if (CurrentUser == null) {return 0;}
        if (CurrentUser.UsersFollowingMe == null) {return 0;}
        return CurrentUser.UsersFollowingMe.Split(',').ToList().Count-1;
    }

    public int GetNumberOfUserFollow(int UserId)
    {
        User CurrentUser = dbContext.Users.Find(UserId);
        if (CurrentUser == null) {return 0;}
        if (CurrentUser.UsersIFollow == null) {return 0;}
        return CurrentUser.UsersIFollow.Split(',').ToList().Count-1;
    }

    public string Feedback = string.Empty;

    [Route("/signup")]
    [HttpGet]
    public IActionResult New()
    {
        var Message = TempData["MESSAGE"];
        ViewBag.Feedback = Feedback;
        Console.WriteLine(ViewBag.Feedback.ToString());
        return View();
    }

    [Route("/users")]
    [HttpPost]
    public RedirectResult Create(User user) {
        bool email_Exists = false;
        Feedback = string.Empty;
        foreach (User u in dbContext.Users)
        {
            if (u.Email == user.Email) 
            {
                email_Exists = true;
                break;
            }
        }
       if (email_Exists) {
        TempData["MESSAGE"] = "Email already assigned to account.";
        return new RedirectResult("/signup");
    } else {
            user.ProfileImage = "https://creativeandcultural.files.wordpress.com/2018/04/default-profile-picture.png?w=256";
            user.UsersIFollow = string.Empty;
            user.UsersFollowingMe = string.Empty;
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            //Then, make go straight to feed.
            HttpContext.Session.SetInt32("user_id", user.Id);
            return new RedirectResult("/posts");
        }
    }

[Route("/profile/{id}")]
[HttpGet]
public IActionResult Profile(int id)
{
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;

    if (currentUserId == null)
    {
        return View();
    } 
    else 
    {
        ViewBag.Id = currentUserId;
        ViewBag.UsersIFollowingNumber = GetNumberOfUserFollow(id);
        ViewBag.UsersFollowersNumber = GetNumberOfUserFollowers(id);
        
        string FollowingStatus = "Follow";
        User currentUser = dbContext.Users.Find(currentUserId);
        User profileUser = dbContext.Users.Find(id);
        
        // ViewBag.currentUsersFollowingMe = currentUser.UsersFollowingMe;
        // ViewBag.profileUsersFollowingMe = profileUser.UsersFollowingMe;
        
        ViewBag.CurrentUserImage = currentUser.ProfileImage;
        
        ViewBag.ProfileLookingAtImage = profileUser.ProfileImage;

            if (currentUser.UsersIFollow.Contains($"'{id}',"))
            {
                FollowingStatus = "Following";
                ViewBag.FollowButtonText = "Unfollow";
            }
            else
            {
                ViewBag.FollowButtonText = "Follow";
            }
                return View(profileUser);
            }
}




[Route("/users/followuser")]
[HttpPost]
public IActionResult Follow(int profileId)
{
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
    User currentUser = dbContext.Users.Find(currentUserId);

    if (!currentUser.UsersIFollow.Contains($"'{profileId}',"))
    {
        // If not already following, follow the user
        FollowUser(profileId);
        ViewBag.FollowButtonText = "Unfollow";
    }
    else
    {
        // If already following, unfollow the user
        UnfollowUser(profileId);
        ViewBag.FollowButtonText = "Follow";
    }

    // Update the ViewBag with the updated follower counts
    ViewBag.UsersIFollowingNumber = GetNumberOfUserFollow(currentUserId);
    ViewBag.UsersFollowersNumber = GetNumberOfUserFollowers(currentUserId);

    // Redirect back to the same profile page
    return RedirectToAction("Profile", new { id = profileId });
}

    [Route("/updateprofile")]
    [HttpPost]

    public RedirectResult UpdateProfile(string newName, string newPhoto, string newBio)
    {
        //AcebookDbContext dbContext = new AcebookDbContext();
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
