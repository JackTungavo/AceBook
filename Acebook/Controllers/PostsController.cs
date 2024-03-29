using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class PostsController : Controller
{
  private readonly ILogger<PostsController> _logger;
  public List<string> ImageFormats = new List<string>() {".jpg",".jpeg",".png"};
  AcebookDbContext dbContext = new AcebookDbContext(); 
  public PostsController(ILogger<PostsController> logger)
  {
    _logger = logger;
  }

  [Route("/posts")]
  [HttpGet]
  public IActionResult Index() 
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId != null && dbContext.Users != null && dbContext.Users.Find(currentUserId) != null) 
    {
      User user = dbContext.Users.Find(currentUserId);
      ViewBag.CurrentUserImage = user.ProfileImage;
      ViewBag.Id = (int)currentUserId.Value;
    }
    
    if (dbContext.Posts != null) 
    {
        List<Post> posts = dbContext.Posts.OrderByDescending(post => post.Time).ToList();
        ViewBag.Posts = posts;
    }

    //ImageFormats
    ViewBag.ImageFormats = ImageFormats;
    return View();
  }

  [Route("/posts")]
  [HttpPost]
  public RedirectResult Create(Post post) 
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId != null) {
      post.UserId = (int)currentUserId.Value;
      post.Time = DateTime.UtcNow;
      post.UsersLiked = string.Empty;
    }

    if (dbContext.Posts != null) 
    {
      dbContext.Posts.Add(post);
      dbContext.SaveChanges();
    }
    return new RedirectResult("/posts");
  }

  [Route("/posts/{id}")]
  [HttpGet]
  public IActionResult Show(int id)
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (dbContext.Posts == null) {return RedirectToAction("Index");}
    Post post = dbContext.Posts.FirstOrDefault(p => p.Id == id) ?? new Post();
    if (post == null)
    {
        // Handle the case where the post with the specified ID is not found
        return RedirectToAction("Index");
    }
    post.Likes = GetLikesFromPost(post);
    List<Post> comments = dbContext.Posts.Where(c => c.PostParentId == id).ToList();
    foreach (Post comment in comments)
    {
      comment.Likes = GetLikesFromPost(comment);
    }
    dbContext.SaveChanges();
    // Pass the post and comments to the view

    List<Post> sortedByTimeComments = comments.OrderByDescending(comment => comment.Time).ToList();
    ViewBag.Post = post;
    ViewBag.Comments = sortedByTimeComments;//comments;
    if (dbContext.Users != null) 
    {
      User? user = dbContext.Users.Find(currentUserId);
      if (user != null) {
        ViewBag.Id = user.Id;
        ViewBag.CurrentUser = user;
        ViewBag.CurrentUserImage = user.ProfileImage;
        ViewBag.ImageFormats = ImageFormats;
        return View(post);
      }
      else
      {
        return new RedirectResult("/signin");
      }
    }
    else //if session currently doesn't exist, take them to signinsignup
    {
      return new RedirectResult("/signin");
    }
  }

  [Route("/posts/comment")]
  [HttpPost]
  public RedirectResult AddComment(int postParentId, Post comment)
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");

    // Comment
    if (dbContext.Posts != null && currentUserId != null) 
    {
      comment.UserId = (int)currentUserId.Value;
      comment.Time = DateTime.UtcNow;
      comment.PostParentId = postParentId;
      comment.UsersLiked = string.Empty;
      dbContext.Posts.Add(comment);
      dbContext.SaveChanges();
    }
    return new RedirectResult($"/posts/{postParentId}");
  }

  [Route("/posts/{id}/like")]
  [HttpPost]

  public RedirectResult LikePost(int id)
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (dbContext.Posts != null && currentUserId != null) 
    {
      Post post = dbContext.Posts.Find(id) ?? new Post();
      post.LikePost((int)currentUserId.Value);
      post.Likes = GetLikesFromPost(post);
      dbContext.Posts.Update(post);
      dbContext.SaveChanges();
      if (post.PostParentId == 0) 
      {
        return new RedirectResult($"/posts/{id}");
      }
      else 
      {
        return new RedirectResult($"/posts/{post.PostParentId}");
      }
    }
    else
    {
      return new RedirectResult($"/posts/{id}");
    }
  }

  
  public int GetLikesFromPost(Post post)
  {
    int finalLikes = 0;
    if (dbContext.Users != null && post.UsersLiked != null) {
      foreach (User user in dbContext.Users)
      {
        if(post.UsersLiked.Contains("'"+user.Id.ToString()+"'"))
          {
            finalLikes ++;
          }
      } 
    }    
    return finalLikes;
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
