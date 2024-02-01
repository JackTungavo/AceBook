using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class PostsController : Controller
{
  private readonly ILogger<PostsController> _logger;
  AcebookDbContext dbContext = new AcebookDbContext(); 

  public PostsController(ILogger<PostsController> logger)
  {
    _logger = logger;
  }

  [Route("/posts")]
  [HttpGet]
  public IActionResult Index() 
  {
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
    List<Post> posts = dbContext.Posts.ToList();
    ViewBag.Posts = posts;
    if (currentUserId != null)
    {
        ViewBag.Id = currentUserId;
    }
    
    return View();
  }

  [Route("/posts")]
  [HttpPost]
  public RedirectResult Create(Post post) 
  {
    //AcebookDbContext dbContext = new AcebookDbContext();
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
    post.UserId = currentUserId;
    post.Time = DateTime.UtcNow;
    post.UsersLiked = string.Empty;
    dbContext.Posts.Add(post);
    dbContext.SaveChanges();
  //   post.PostParentId = post.Id;
  //   dbContext.SaveChanges();

    return new RedirectResult("/posts");
  }

  [Route("/posts/{id}")]
  [HttpGet]
  public IActionResult Show(int id)
  {
  //AcebookDbContext dbContext = new AcebookDbContext();
  Post post = dbContext.Posts.FirstOrDefault(p => p.Id == id);
  post.Likes = GetLikesFromPost(post);

  if (post == null)
  {
      // Handle the case where the post with the specified ID is not found
      return RedirectToAction("Index");
  }

  List<Post> comments = dbContext.Posts.Where(c => c.PostParentId == id).ToList();
  foreach (Post comment in comments)
  {
    comment.Likes = GetLikesFromPost(comment);
  }
  dbContext.SaveChanges();
  // Pass the post and comments to the view

  List<Post> sortedByTimeCommments = comments.OrderByDescending(comment => comment.Time).ToList();
  ViewBag.Post = post;
  ViewBag.Comments = sortedByTimeCommments;//comments;

  return View(post);
  
}

  [Route("/posts/comment")]
  [HttpPost]
  public RedirectResult AddComment(int postParentId, Post comment)
  {
    //AcebookDbContext dbContext = new AcebookDbContext();
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;

    // Comment
    comment.UserId = currentUserId;
    comment.Time = DateTime.UtcNow;
    comment.PostParentId = postParentId;
    comment.UsersLiked = string.Empty;
    dbContext.Posts.Add(comment);
    dbContext.SaveChanges();

    return new RedirectResult($"/posts/{postParentId}");
  }

  [Route("/posts/{id}/like")]
  [HttpPost]

  public RedirectResult LikePost(int id)
  {
    //AcebookDbContext dbContext = new AcebookDbContext();
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
    Post post = dbContext.Posts.Find(id);
    post.LikePost(currentUserId);
    post.Likes = GetLikesFromPost(post);
    dbContext.Posts.Update(post);
    dbContext.SaveChanges();
    if (post.PostParentId == 0) {return new RedirectResult($"/posts/{id}");}
    else {return new RedirectResult($"/posts/{post.PostParentId}");}
  }

  
  public int GetLikesFromPost(Post post)
  {
    //AcebookDbContext dbContext = new AcebookDbContext();
    int finalLikes = 0;

    foreach (User user in dbContext.Users)
    {
      if(post.UsersLiked.Contains("'"+user.Id.ToString()+"'"))
        {
          finalLikes ++;
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
