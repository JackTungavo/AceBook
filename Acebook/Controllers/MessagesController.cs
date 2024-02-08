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
public class MessagesController : Controller
{
  private readonly ILogger<PostsController> _logger;
  AcebookDbContext dbContext = new AcebookDbContext(); 
  public MessagesController(ILogger<PostsController> logger)
  {
    _logger = logger;
  }

  public bool CheckTwoUsersAreFriends(int id1, int id2)
  {
    bool result = false;
    if (dbContext.Users == null) {return false;}
    User? U1 = dbContext.Users.Find(id1);
    User? U2 = dbContext.Users.Find(id2);

    if (U1 != null && U2 != null && U1.UsersIFollow != null && U2.UsersIFollow != null)
    {
      if (U1.UsersIFollow.Contains(id2.ToString()) && U2.UsersIFollow.Contains(id1.ToString())) 
      {
        result = true;
      }
    }

    return result;
  }

  public Tuple<bool, string, Message> CheckMessagesExistBetweenTwo(int userid1, int userid2)
  {
    bool finalresult = false;
    string LastMessage = "";
    Message MessageObj = new();

    if (dbContext.Messages != null) 
    {
      var AllMessages = dbContext.Messages.ToList();
      AllMessages = AllMessages.OrderBy(m => m.Time).ToList();
      foreach (Message m in AllMessages)
      {
        if (m.User1Id == userid1 && m.User2Id == userid2 && m.Content != null) {finalresult = true; LastMessage = m.Content; MessageObj = m; break;}
        else if (m.User1Id == userid2 && m.User2Id == userid1 && m.Content != null) {finalresult = true; LastMessage = m.Content; MessageObj = m; break;}
      }
    }

    return Tuple.Create(finalresult, LastMessage, MessageObj);
  }

  [Route("/messages")]
  [HttpGet]
  public IActionResult Index() 
  {
    //returns if we have conversations with users
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId == null) {return new RedirectResult("/signin");}
    if (dbContext.Users != null && currentUserId != null)
    {
      List<List<dynamic>> HasConversation = new List<List<dynamic>>(); //[0] = OtherUserObj, [1] = Any Messages Sent Bool, [2] = Last Message Sent
      ViewBag.Id = currentUserId.Value;
      var AllUsers = dbContext.Users.ToList();
      foreach (User otherUser in AllUsers)
      {
        if (currentUserId != null && otherUser.Id != (int)currentUserId) // if user checking is not current user
        {
          if (CheckTwoUsersAreFriends((int)currentUserId, otherUser.Id) == true) 
          {
            Tuple<bool, string, Message> CheckBoolAndLastMessage = CheckMessagesExistBetweenTwo((int)currentUserId, otherUser.Id);
            HasConversation.Add(new List<dynamic> {
              otherUser,                     //OtherUserObject
              CheckBoolAndLastMessage.Item1, //Messages Sent Boolean
              CheckBoolAndLastMessage.Item2, //Last Message Sent String
              CheckBoolAndLastMessage.Item3  //Last MessageObject
              });
          }
        }
      }
    
      //if conversation exists, make hyperlink going to "messages/userid" for conversation
      User? currentUser = dbContext.Users.Find(currentUserId);
      if (currentUser != null) 
      {
        ViewBag.currentUser = currentUser; 
        ViewBag.currentUserId = currentUserId;
        ViewBag.CurrentUserImage = currentUser.ProfileImage;
      }
      ViewBag.HasConversation = HasConversation;
      return View();
    }
    else {return new RedirectResult("/signin");}
  }

  [Route("/messages/{OtherUserId}")]
  [HttpGet]
  public IActionResult Show(int OtherUserId)
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId == null || dbContext.Messages == null || dbContext.Users == null)  {return new RedirectResult("/signin");}
    else 
    {
    ViewBag.Id = currentUserId.Value;
    List<Message> AllMessages = dbContext.Messages.ToList();
    List<Message> FinalMessages = dbContext.Messages.ToList();
    foreach (Message m in AllMessages) {
      bool validMessage = true;
      //check if both user id's don't match current user
      if (m.User1Id != (int)currentUserId && m.User2Id != (int)currentUserId){validMessage = false;}
      //check if both user if's don't match other user
      if (m.User1Id != OtherUserId && m.User2Id != OtherUserId) {validMessage = false;}
      if (validMessage == false) {FinalMessages.Remove(m);}
    }
    
    ViewBag.AllMessages = FinalMessages.OrderByDescending(m => m.Time).ToList();
    User? currentUser = dbContext.Users.Find(currentUserId);
    User? otherUser = dbContext.Users.Find(OtherUserId);

    if (currentUser != null) 
    {
      ViewBag.CurrentUserImage = currentUser.ProfileImage;
      ViewBag.currentUser = currentUser;
    }

    if (otherUser != null) {ViewBag.otherUser = otherUser;}
    
    return View();
    }
  }

  [Route("/messages/{OtherUserId}")]
  [HttpPost]
  public RedirectResult SendMessage(int OtherUserId)
  {
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (dbContext.Messages != null && currentUserId != null) 
    {
      Message newMessage = new();
      newMessage.Content = Request.Form["content"];;
      newMessage.User1Id = (int)currentUserId.Value;
      newMessage.User2Id = OtherUserId;
      newMessage.Time = DateTime.UtcNow;
      if (newMessage.Content != string.Empty) 
      {
        dbContext.Messages.Add(newMessage);
        dbContext.SaveChanges();
      }
    }
    return new RedirectResult($"/messages/{OtherUserId}");
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
